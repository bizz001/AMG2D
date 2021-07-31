using AMG2D.Model.Persistence;

namespace AMG2D.Model
{
    public interface IExternalObjectsPositioner
    {
        void PositionExternalObjects(ref MapPersistence map);
    }
}