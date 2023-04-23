namespace MagicVilla_CouponAPI.Models.DTO
{
    public class CouponCreateDTO
    {
        //it is telling what data user can add
        public string Name { get; set; }
        public int Percent { get; set; }
        public bool IsActive { get; set; }
    }
}
