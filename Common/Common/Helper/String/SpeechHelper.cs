using System;
using System.Collections.Generic;
using System.Speech.Synthesis;
using System.Text;

namespace Common
{
  public static class SpeechHelper
  {
    static SpeechSynthesizer Synth;
    static SynthesizerState _state = SynthesizerState.Ready;
    static List<string> _speechs = new List<string>();
    static SpeechHelper()
    {
      Synth = new SpeechSynthesizer();
      Synth.StateChanged += Synth_StateChanged;
      Synth.Rate = -2;
    }

    private static void Synth_StateChanged(object sender, StateChangedEventArgs e)
    {
      lock (_speechs)
      {
        _state = e.State;
        if (_state == SynthesizerState.Ready)
        {
          if (_speechs.Count > 0)
          {
            var s = _speechs[0];
            _speechs.RemoveAt(0);
            Synth.SpeakAsync(s);
          }
        }
      }
    }
    public static void Clear()
    {
      lock (_speechs)
      {
        _speechs.Clear();
        if (_state == SynthesizerState.Speaking)
        {
          Synth.SpeakAsyncCancelAll();
        }
      }
    }
    public static void Speech(string SpeechForm)
    {
      if (!string.IsNullOrEmpty(SpeechForm))
      {
        var s = Convert(SpeechForm);

        lock (_speechs)
        {
          if (_state == SynthesizerState.Speaking)
          {
            _speechs.Add(s);
            return;
          }
        }

        try
        {
          Synth.SpeakAsync(s);
        }
        catch (Exception ex)
        {
          LOG.except(ex);
        }
      }
    }

    static string Convert(string s)
    {
      var sb = new StringBuilder();
      bool number_mode = false;
      List<char> numbers = new List<char>();
      foreach (char c in s)
      {
        if (char.IsDigit(c))
        {
          if (!number_mode)
          {
            number_mode = true;
          }
          numbers.Add(c);
        }
        else
        {
          if (number_mode)
          {
            var n = GetNumberString(string.Join("", numbers));
            number_mode = false;
            numbers.Clear();
            sb.Append(n);
          }
          sb.Append(c);
        }
      }
      return sb.ToString();
    }
    static string GetNumberString(string s)
    {
      s = s.PadLeft(4, '0'); // 최대 4자리. 45 => 0045

      var sb = new StringBuilder();
      int idx = 0;
      foreach (char c in s)
      {
        int k = (int)Char.GetNumericValue(c); // '2' => 2
        sb.Append(Hans[idx, k]);
        if (++idx > 3)
        {
          break;
        }
      }
      return sb.ToString();
    }

    static string[,] Hans = new string[,] {
            { "", "천", "이천", "삼천", "사천", "오천", "육천", "칠천", "팔천", "구천" },
            { "", "백", "이백", "삼백", "사백", "오백", "육백", "칠백", "팔백", "구백" },
            { "", "십", "이십", "삼십", "사십", "오십", "육십", "칠십", "팔십", "구십" },
            { "", "일", "이", "삼", "사", "오", "육", "칠", "팔", "구" }
        };
  }
}