namespace Domain.Entities
{
    public interface IBaseModel<KeyType>
    {
        KeyType Id { get; set; }
        DateTime CreateDateTime { get; set; }
    }
}
