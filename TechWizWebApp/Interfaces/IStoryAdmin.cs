using TechWizWebApp.Data;
using TechWizWebApp.Domain;

namespace TechWizWebApp.Interfaces
{
    public interface IStoryAdmin
    {
        public Task<CustomResult> CreateNewStory(Story story);

        public Task<CustomPaging> GetDesignerStories(int designer_id, int pageNumber, int pageSize, string from, string to);

        public Task<CustomResult> UpdateStory(Story story);

    }
}
