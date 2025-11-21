// EnemyHealthSystem.cs
public class EnemyHealthSystem
{
    public int CurrentHP { get; private set; }
    public bool IsDead { get; private set; }

    // Constructor: Khởi tạo máu
    public EnemyHealthSystem(int maxHP)
    {
        CurrentHP = maxHP;
        IsDead = false;
    }

    // Hàm tính toán sát thương (Trả về true nếu vừa mới chết)
    public bool TakeDamage(int amount)
    {
        if (IsDead) return false;

        CurrentHP -= amount;

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            IsDead = true;
            return true; // Báo hiệu là enemy vừa chết
        }
        
        return false; // Vẫn còn sống
    }
}