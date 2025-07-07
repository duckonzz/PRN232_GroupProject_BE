using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GenderHealthCare.ModelViews.BlogModels;

namespace GenderHealthCare.Contract.Services.Interfaces
{
    public interface IBlogService
    {
        Task<BlogResponseModel> CreateBlogAsync(BlogRequestModel model, string authorId);
        Task<BlogResponseModel?> GetBlogByIdAsync(string id);
        Task<List<BlogResponseModel>> GetAllBlogsAsync();
        Task<bool> UpdateBlogAsync(string id, BlogRequestModel model);
        Task<bool> DeleteBlogAsync(string id);
    }
}
