using Santander.API.Common.Model;

namespace Santander.API.Common.Interface
{
    public interface IBestStoryTransformer<T>
    {
        Story Transform(T storyResponse);
    }
}
