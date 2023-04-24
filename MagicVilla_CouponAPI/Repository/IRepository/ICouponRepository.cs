using MagicVilla_CouponAPI.Models;

namespace MagicVilla_CouponAPI.Repository.IRepository
{
    public interface ICouponRepository
    {
        Task CreateAsync(Coupon coupon);
        Task UpdateAsync(Coupon coupon);
        Task DeleteAsync(Coupon coupon);
        Task SaveAsync();
        Task<Coupon> GetAsync(int id);
        Task<Coupon> GetAsync(string CouponName);
        Task<ICollection<Coupon>> GetAllAsync();
    }
}
