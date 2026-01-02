public class RewardData
{
    public RewardCategory Category; //보상 대분류
    public RewardType RewardType;   //아이템 타입 (Item일 때만 사용)
    public int Amount;              //수량 (Item일 때만 사용)
    public EggData Egg;             //알 데이터 (Egg일 때만 사용)

    // 아이템 보상용 생성자
    public static RewardData CreateItem(RewardType type, int amount)
    {
        return new RewardData
        {
            Category = RewardCategory.Item,
            RewardType = type,
            Amount = amount
        };
    }

    // 알 보상용 생성자
    public static RewardData CreateEgg(EggData egg)
    {
        return new RewardData
        {
            Category = RewardCategory.Egg,
            Egg = egg
        };
    }
}