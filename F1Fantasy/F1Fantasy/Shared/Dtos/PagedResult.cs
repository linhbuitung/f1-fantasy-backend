namespace F1Fantasy.Shared.Dtos;

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; }
    public int TotalCount { get; set; }
    public int PageNum { get; set; }
    public int PageSize { get; set; }
}