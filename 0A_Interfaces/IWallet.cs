public interface IWallet
{
    void ChangeBalance(int points);
    bool HasEnoughPoints(int points);
    bool IsBroke();
}
