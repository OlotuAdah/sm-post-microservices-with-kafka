
using Post.Common.DTO;
using Post.Query.Domain.Entities;

namespace Post.Query.Api.DTO;

public class PostLookupResponse : BaseResponse
{
    public List<PostEntity> Posts { get; set; }
}