namespace Web.Api.Dtos.Outcomming.RentalDto
{
    public class RentalListDto
    {
        public IReadOnlyList<RentalDto> RentalsPaginationList { get; set; }

        public int Count { get; set; }
    }
}
