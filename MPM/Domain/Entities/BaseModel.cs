namespace Domain.Entities
{
    public abstract class BaseModel<KeyType> : IBaseModel<KeyType>
    {
        protected BaseModel()
        {
            CreateDateTime = DateTime.Now;
        }
        public KeyType Id { get; set; }
        public DateTime CreateDateTime { get; set; }
    }
    public abstract class BaseModel : BaseModel<long>, IBaseModel<long>
    {
    }
}
