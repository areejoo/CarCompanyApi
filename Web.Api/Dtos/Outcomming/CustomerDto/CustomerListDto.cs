namespace Web.Api.Dtos.Outcomming.CustomerDto
{
    public class CustomerListDto
    {
        public IReadOnlyList<CustomerDto> CustomerPaginationList { get; set; }

        public int Count { get; set; }
    }
}
