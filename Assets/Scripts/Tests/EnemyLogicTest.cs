using NUnit.Framework;

public class EnemyLogicTest
{
    [Test]
    public void TestTakeDamage()
    {
        
        var healthSystem = new EnemyHealthSystem(100);

        
        healthSystem.TakeDamage(10);

        
        Assert.AreEqual(90, healthSystem.CurrentHP);
        Assert.IsFalse(healthSystem.IsDead);
    }

    [Test]
    public void TestDie()
    {
        var healthSystem = new EnemyHealthSystem(10);
        
        
        bool justDied = healthSystem.TakeDamage(10);

        Assert.AreEqual(0, healthSystem.CurrentHP);
        Assert.IsTrue(healthSystem.IsDead);
        Assert.IsTrue(justDied);
    }

    [Test]
    public void TestNoDamageWhenDead()
    {
        
        var healthSystem = new EnemyHealthSystem(1);
        healthSystem.TakeDamage(5); 
        
        int initialHP = healthSystem.CurrentHP; 

       
        bool justDiedAgain = healthSystem.TakeDamage(50); 

        
        Assert.AreEqual(initialHP, healthSystem.CurrentHP, "HP không nên thay đổi sau khi đã chết.");
        Assert.IsFalse(justDiedAgain, "Hàm phải trả về false vì enemy đã chết từ trước.");
        Assert.IsTrue(healthSystem.IsDead, "Trạng thái IsDead phải vẫn là true.");
    }
}