using Common;
using UIControls;
using System.Collections.Concurrent;
using System.IO;
using System.Media;
using System.Threading;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EUMC.Client
{
  public class CallMessageVM : ViewModelBase
  {
    // call patient
    public bool IsLandscape { get; set; }
    public bool MinPopup { get; set; }
    public bool ShowPopup { get => _showPopup; set => Set(ref _showPopup, value); }
    public string PopupPatientName { get => _popupPatientName; set => Set(ref _popupPatientName, value); }
    public string PopupMessage { get => _popupMessage; set => Set(ref _popupMessage, value); }

    // operation information
    public bool ShowOperation { get => _showOperation; set => Set(ref _showOperation, value); }
    public string OpMessage1 { get => _opMessage1; set => Set(ref _opMessage1, value); }
    public string OpMessage2 { get => _opMessage2; set => Set(ref _opMessage2, value); }

    int _callPopupDuration = 10; // seconds

    /// <summary>
    /// call popup
    /// </summary>
    /// <param name="isLandscape">landscape 여부</param>
    /// <param name="minPopup">작은 사이즈</param>
    /// <param name="call_popup_duration">팝업 지속 시간</param>
    public CallMessageVM(bool isLandscape, bool minPopup = false, int call_popup_duration = 10)
    {
      this.IsLandscape = isLandscape;
      this.MinPopup = minPopup;
      this._callPopupDuration = call_popup_duration;
    }
    public void Announce(string speech) => this.enqueue_message(new ANNOUNCER_MESSAGE(speech));
    public void CallMessage(string name, string message, string speech) => this.enqueue_message(new CALL_MESSAGE(name, message, speech));
    public void OperationMessage(string msg1, string msg2) => this.enqueue_message(new OPERATION_MESSAGE(msg1, msg2));
    void enqueue_message(MESSAGE_BASE msg)
    {
      if (this.IsRunning)
      {
        this.AddMessage(msg);
      }
      else
      {
        this.show_call_message(msg);
      }
    }
    void show_call_message(MESSAGE_BASE msg)
    {
      LOG.dc($"show message");
      Interlocked.Exchange(ref _is_running, 1);

      switch (msg.Type)
      {
        case MessageType.Call:
          {
            var m = msg.CastTo<CALL_MESSAGE>();
            this.ShowPopup = true;
            this.PopupPatientName = m.PatientName;
            this.PopupMessage = m.Message;
            SpeechHelper.Speech(m.Speech);
            TimerHelper.RunOnce(() => { this.call_completed(); }, _callPopupDuration);
          }
          break;
        case MessageType.Operation:
          {
            var m = msg.CastTo<OPERATION_MESSAGE>();
            this.ShowOperation = true;
            this.OpMessage1 = m.Message1;
            this.OpMessage2 = m.Message2;
            SpeechHelper.Speech(m.Speech);
            TimerHelper.RunOnce(() => { this.call_completed(); }, 20);
          }
          break;
        case MessageType.Announce:
          {
            var m = msg.CastTo<ANNOUNCER_MESSAGE>();
            SpeechHelper.Speech(m.Speech);
            TimerHelper.RunOnce(() => { this.announce_completed(); }, 5);
          }
          break;
        default:
          return;
      }
      _currentMessage = msg;
    }

    void call_completed()
    {
      Interlocked.Exchange(ref _is_running, 0);
      _currentMessage = null;

      this.ShowPopup = false;
      this.ShowOperation = false;
      this.OpMessage1 = string.Empty;
      this.OpMessage2 = string.Empty;

      var msg = this.GetNextMessage();
      if (msg != null)
      {
        this.show_call_message(msg);
      }
    }
    void announce_completed()
    {
      Interlocked.Exchange(ref _is_running, 0);
      _currentMessage = null;

      var msg = this.GetNextMessage();
      if (msg != null)
      {
        this.show_call_message(msg);
      }
    }

    MESSAGE_BASE GetNextMessage()
    {
      lock (_messages)
      {
        if(_messages.Any())
          return _messages.Dequeue();
        return null;
      }
    }
    void AddMessage(MESSAGE_BASE msg)
    {
      lock (_messages)
      {
        if (_currentMessage != null && _currentMessage.Equals(msg))
        {
          LOG.ec("dup current message");
          return;
        }

        if (_messages.ToList().Find(x => x.Equals(msg)) != null)
        {
          LOG.ec("dup exist message");
          return;
        }

        _messages.Enqueue(msg);
      }
    }

    public void Close()
    {
      _messages.Clear();

      SpeechHelper.Clear();
      Interlocked.Exchange(ref _is_running, 0);
      this.ShowPopup = false;
      this.ShowOperation = false;
      this.OpMessage1 = string.Empty;
      this.OpMessage2 = string.Empty;
    }

    public void Bell()
    {
      var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources", "Sound", "Doorbell.WAV");

      if (!File.Exists(path))
      {
        LOG.ec($"file not found {path}");
        return;
      }

      var player = new SoundPlayer(path);
      player.Play();
    }

    public bool IsRunning => Interlocked.Read(ref _is_running) == 1;
    Queue<MESSAGE_BASE> _messages = new Queue<MESSAGE_BASE>();

    MESSAGE_BASE _currentMessage = null;

    // call message
    bool _showPopup;
    string _popupPatientName = string.Empty;
    string _popupMessage= string.Empty;
    long _is_running = 0;

    // operation message
    bool _showOperation;
    string _opMessage1 = string.Empty;
    string _opMessage2  = string.Empty;

    public enum MessageType
    {
      Call,
      Operation,
      Announce,
    }

    public class MESSAGE_BASE : IEquatable<MESSAGE_BASE>
    {
      public MessageType Type { get; set; }
      public string Speech { get; set; } = string.Empty;
      public MESSAGE_BASE(MessageType type, string speech)
      {
        this.Type = type;
        this.Speech = speech;
      }

      public bool Equals(MESSAGE_BASE other)
      {
        if (other == null) return false;
        return this.Type == other.Type && this.Speech == other.Speech;
      }
      public override int GetHashCode()
      {
        return (int)this.Type + this.Speech.GetHashCode();
      }
    }
    public class ANNOUNCER_MESSAGE : MESSAGE_BASE
    {
      public ANNOUNCER_MESSAGE(string speech) : base(MessageType.Announce, speech)
      {
      }
    }
    public class CALL_MESSAGE : MESSAGE_BASE
    {
      public string PatientName { get; set; } = string.Empty;
      public string Message { get; set; } = string.Empty;
      public CALL_MESSAGE(string name, string message, string speech) : base(MessageType.Call, speech)
      {
        this.PatientName = name;
        this.Message = message;
      }
    }
    public class OPERATION_MESSAGE : MESSAGE_BASE
    {
      public string Message1 { get; set; } = string.Empty;
      public string Message2 { get; set; } = string.Empty;
      public OPERATION_MESSAGE(string msg1, string msg2) : base(MessageType.Operation, $"{msg1} {msg2}")
      {
        this.Message1 = msg1;
        this.Message2 = msg2;
      }
    }
  }
}