

public class BodyEquipment : Equipment
{
    public static BodyEquipment None()
    {
        return new BodyEquipment {Name = "None"};
    }

    public static BodyEquipment Leather()
    {
        return new BodyEquipment()
        {
            Defense = 1,
            DefenseChance = 10,
            Name = "LeatherA"
        };
    }

    public static BodyEquipment Chain()
    {
        return new BodyEquipment()
        {
            Defense = 2,
            DefenseChance = 5,
            Name = "ChainA"
        };
    }

    public static BodyEquipment Plate()
    {
        return new BodyEquipment()
        {
            Defense = 2,
            DefenseChance = 10,
            Name = "PlateA"
        };
    }

}