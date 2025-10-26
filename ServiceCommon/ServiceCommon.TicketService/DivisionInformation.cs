namespace ServiceCommon.TicketService
{
  internal class DivisionInformation
  {
    public int DivisionID;
    public int StartNo;
    public int EndNo;
    public string DivisionName = string.Empty;
    public string Description = string.Empty;

    int _nextNo;

    public DivisionInformation(int div, int start, int end, string name, string desc, int current_no = 0)
    {
      this.DivisionID = div;
      this.StartNo = start;
      this.EndNo = end;

      _nextNo = this.StartNo;
      if (this.StartNo < current_no && current_no <= this.EndNo)
      {
        this._nextNo = current_no;
      }
    }

    public int GetNextNo()
    {
      // 여러 발권기에서 같은 DivId 를 사용할수있지만, 요청을 동기적으로 처리되므로 lock 불필요
      //lock (LOCK)
      {
        int no = _nextNo++;

        if (_nextNo > this.EndNo)
        {
          _nextNo = this.StartNo;
        }
        return no;
      }
    }
  }
}