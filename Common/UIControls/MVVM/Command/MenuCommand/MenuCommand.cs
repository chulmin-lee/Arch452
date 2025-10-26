using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIControls
{
  public class MenuCommand : RelayCommand
  {
    public MenuCommand(int id, Action<object> execute, string header, string shortcut = null) : base(execute, header, shortcut)
    {
      this.ID = id;
    }

    public MenuCommand(int id, Action<object> execute, Predicate<object> canExecute, string header, string shortcut = null) : base(execute, canExecute, header, shortcut)
    {
      this.ID = id;
    }

    public override void Execute(object parameter)
    {
      if (parameter == null)
        parameter = ID;
      _execute(parameter);
    }

    #region BUILDER
    public class Builder
    {
      int _id;
      string _name;
      string _shortcut;
      Predicate<object> _canExecute;
      Action<object> _execute;

      public Builder()
      {
      }
      public Builder(int id)
      {
        this._id = id;
      }
      public Builder(int id, string name)
      {
        this._id = id;
        this._name = name;
      }
      public Builder(int id, string name, string shortcut)
      {
        this._id = id;
        this._name = name;
        this._shortcut = shortcut;
      }
      //public Builder((int id, string name, string shortcut) t)
      //{
      //  this._id = t.id;
      //  this._name = t.name;
      //  this._shortcut = t.shortcut;
      //}
      public Builder ID(int id)
      {
        this._id = id;
        return this;
      }
      public Builder Name(string name)
      {
        this._name = name;
        return this;
      }
      public Builder Shortcut(string shortcut)
      {
        this._shortcut = shortcut;
        return this;
      }
      public Builder When(Predicate<object> canExecute)
      {
        this._canExecute = canExecute;
        return this;
      }
      public Builder Exec(Action<object> execute)
      {
        this._execute = execute;
        return this;
      }

      public MenuCommand Build()
      {
        return new MenuCommand(_id, _execute, _canExecute, _name, _shortcut);
      }
    }
    #endregion
  }
}
