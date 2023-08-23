namespace Web.Api.Dtos.Outcomming.DriverDto
{
    public class DriverListDto
    {
        public IReadOnlyList<DriverDto> DriverPaginationList { get; set; }

        public int Count { get; set; }
    }
}
