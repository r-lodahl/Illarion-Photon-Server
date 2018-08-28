namespace Illarion.Server
{
    public interface INavigationManager
    {
        INavigator GetNavigator(int worldIndex);
    }
}