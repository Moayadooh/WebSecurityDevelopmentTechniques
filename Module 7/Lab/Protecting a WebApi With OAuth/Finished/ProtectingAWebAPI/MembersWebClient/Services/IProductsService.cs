namespace MembersWebClient.Services
{
  public interface IProductsService
  {
    Task<List<string>> GetProductsAsync();
  }
}
