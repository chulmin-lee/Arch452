using System.Collections.Generic;
using System.Linq;

namespace Common
{
  public static class PageHelper
  {
    /// <summary>
    /// 페이지를 꽉채우는 최대 아이템 갯수
    /// </summary>
    /// <param name="total_item">전체 아이템 갯수</param>
    /// <param name="page_item_count">페이지 당 아이템 갯수</param>
    /// <returns></returns>
    public static int CalcMaxItemCount(this int total_item, int page_item_count)
    {
      return CalcPageCount(total_item, page_item_count) * page_item_count;
    }
    public static int CalcMaxItemCount<T>(this List<T> items, int page_item_count) => CalcPageCount(items.Count(), page_item_count);
    /// <summary>
    /// 최대 페이지 수 계산
    /// </summary>
    /// <param name="total_item">전체 아이템 갯수</param>
    /// <param name="page_item_count">페이지 당 아이템 갯수</param>
    /// <returns>페이지 수</returns>
    public static int CalcPageCount(this int total_item, int page_item_count)
    {
      if (page_item_count > 0 && total_item > 0)
      {
        return (total_item / page_item_count) + ((total_item % page_item_count) > 0 ? 1 : 0);
      }
      return 0;
    }
    public static int CalcPageCount<T>(this List<T> items, int page_item_count) => CalcPageCount(items.Count(), page_item_count);

    /// <summary>
    /// 전체 그룹이 순환할때 순환 카운터에 해당하는 그룹 인덱스 구하기
    /// 예) 순환주기마다 5개의 그룹을 차례로 보여줄때 사용
    /// </summary>
    /// <param name="total_page">페이지 갯수</param>
    /// <param name="loop_counter">계속 증가한다</param>
    /// <returns>페이지는 0부터 시작</returns>
    public static int GetCurrentPage(this int total_page, int loop_counter)
    {
      if (total_page <= 0) return 0;
      return loop_counter % total_page;
    }

    /// <summary>
    /// 순환주기중 전체 목록갯수가 변경되었을때 현재 그룹 인덱스
    /// </summary>
    /// <param name="total_item">전체 item 갯수</param>
    /// <param name="page_item_count">페이지당 item 갯수</param>
    /// <param name="loop_counter"></param>
    /// <returns></returns>
    public static int GetCurrentPage(this int total_item, int page_item_count, int loop_counter)
    {
      int total_page = CalcPageCount(total_item, page_item_count);
      return GetCurrentPage(total_page, loop_counter);
    }
    public static int GetCurrentPage<T>(this List<T> items, int page_item_count, int loop_counter)
    => GetCurrentPage(items.Count(), page_item_count, loop_counter);

    /// <summary>
    /// 전체 목록에서 지정된 페이지에 해당하는 아이템 리턴
    /// </summary>
    /// <param name="items">전체 목록</param>
    /// <param name="page_no">페이지 번호 (0부터 시작)</param>
    /// <param name="page_item_count">페이지당 아이템수</param>
    public static List<T> GetPageItems<T>(this List<T> items, int page_no, int page_item_count)
    {
      if (page_item_count <= 0) return new List<T>();
      if (items.Count() <= page_item_count) return items.ToList();

      // 1. 전체 페이지 수 구하기
      var page_count = items.CalcPageCount(page_item_count);

      if (page_count <= page_no)
      {
        page_no = page_count - 1; // 마지막 페이지로 이동
      }

      int skip_count = page_no * page_item_count;
      return items.Skip(skip_count).Take(page_item_count).ToList();
    }

    /// <summary>
    /// loop_counter에 해당하는 페이지 아이템 리턴
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="items">전체 목록</param>
    /// <param name="loop_counter">계속 증가하는 값</param>
    /// <param name="page_item_count">페이지당 아이템수 (읽어야할 갯수)</param>
    /// <param name="page_no">현재 페이지 번호</param>
    public static List<T> GetPageItems<T>(this List<T> items, int page_item_count, int loop_counter, out int page_no)
    {
      page_no = GetCurrentPage(items, page_item_count, loop_counter);
      return GetPageItems(items, page_no, page_item_count);
    }
  }
}