namespace NewsPaper.src.Domain.Entities
{
    public class ChildrenCategory : BaseEntity
    {
        public int ChildrenCategoryId { get; set; }
        public string ChildrenCategoryName { get; set; }
        public int ParentCategoryId { get; set; }   
        public string Description { get; set; }
    }
}
