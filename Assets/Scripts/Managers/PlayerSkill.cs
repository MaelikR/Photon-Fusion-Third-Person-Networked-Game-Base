public class PlayerSkill
{
    public string skillName;
    public int manaCost;
    public float cooldown;
    public int damage;
    public bool isUnlocked;
    public float cooldownRemaining;

    // Constructeur qui accepte 4 arguments
    public PlayerSkill(string name, int cost, float cd, int dmg)
    {
        skillName = name;
        manaCost = cost;
        cooldown = cd;
        damage = dmg;
        isUnlocked = false; // Par défaut, la compétence est verrouillée
        cooldownRemaining = 0f;
    }
}
