namespace Illarion.Server
{
    public interface INavigationManager
    {
        INavigator GetNavigator(IWorld world);
    }
}