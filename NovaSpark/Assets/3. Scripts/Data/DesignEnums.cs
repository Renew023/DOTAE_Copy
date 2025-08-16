public static class DesignEnums {
    public enum Rarity {
        Common = 0,
        UnCommon = 1,
        Rare = 2,
        Epic = 3,
        Legendary = 4,
        None = 5,
    }
    public enum EffectType {
        Heal = 0,
        AttackBuff = 1,
        DefenceBuff = 2,
        AttackSpeedBuff = 3,
        MoveSpeedBuff = 4,
        None = 5,
        thirsty = 6,
        hunger = 7,
        UnlockRecipe = 8,
    }
    public enum StatType {
        Hp = 0,
        Damage = 1,
        Speed = 2,
        Defence = 3,
        AttackSpeed = 4,
        AttackRange = 5,
        CriticalPercent = 6,
        Accuracy = 7,
        Missing = 8,
        None = 9,
    }
    public enum EquipmentType {
        Weapon = 0,
        Armor = 1,
        Head = 2,
        Arm = 3,
        Leg = 4,
        None = 5,
    }
    public enum MaterialUse {
        Common = 0,
        Enhance = 1,
        Enchant = 2,
        Alchemy = 3,
        Craft = 4,
        None = 5,
    }
    public enum SkillType {
        Attack = 0,
        Buff = 1,
        None = 2,
    }
    public enum DialogueType {
        Basic = 0,
        PlayerChoice = 1,
        Quest = 2,
        None = 3,
    }
    public enum BodyPartType {
        Blood = 0,
        Head = 1,
        Body = 2,
        Arm = 3,
        Leg = 4,
        None = 5,
    }
    public enum Ability {
        None = 0,
        Run = 1,
        Regeneration = 2,
        Make = 3,
        UnLock = 4,
        Battle = 5,
        Poison_Bite = 6,
        Howl = 7,
        WebShot = 8,
        Slash = 9,
        Ambush = 10,
        Acid_Splash = 11,
        Bite = 12,
        Still = 13,
    }
    public enum VillageType {
        DragonRare = 0,
        CasinoVill = 1,
        CastleVill = 2,
        RookieVill = 3,
        UndeadVill = 4,
        OrcVill = 5,
        None = 6,
        Cemetery = 7,
        SosuVill = 8,
        TrainTrader = 9,
        Gang = 10,
    }
    public enum BloodType {
        Person = 0,
        Dragon = 1,
        UnDead = 2,
        Beast = 3,
        Elemental = 4,
        Hollow = 5,
        None = 6,
    }
    public enum CharacterType {
        Player = 0,
        Enemy = 1,
        npc = 2,
        None = 3,
    }
    public enum PlaceType {
        HaHa = 0,
        GaGa = 1,
        BaBa = 2,
        Field = 3,
        None = 4,
    }
    public enum NPCType {
        None = 0,
        FarmingNPC = 1,
        FishingNPC = 2,
        MiningNPC = 3,
        CutterNPC = 4,
        EnhanceNPC = 5,
        CasinoNPC = 6,
        EnchantNPC = 7,
        WeaponNPC = 8,
        PositionNPC = 9,
        OtherNPC = 10,
        TrainerNPC = 11,
        QuestNPC = 12,
        HotelNPC = 13,
        DeadBodyNPC = 14,
        ArmorNPC = 15,
        MaterialNPC = 16,
    }
    public enum RiggingType {
        None = 0,
        Humanoid = 1,
        Leg4 = 2,
    }
    public enum QuestType {
        None = 0,
        Main = 1,
        Sub = 2,
        Repeat = 3,
    }
    public enum QuestEvent {
        None = 0,
        Talk = 1,
        collect = 2,
        Arrival = 3,
        Kill = 4,
        Enhance = 5,
    }
}
