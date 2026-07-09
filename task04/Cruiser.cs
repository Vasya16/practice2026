namespace task04;

public class Cruiser : ISpaceship
{
    private int _position = 0;
    private int _currentAngle = 0;

    private int _rocketsCount = 10;

    public int Speed => 50;
    public int FirePower => 100;

    public void MoveForward()
    {
        _position += Speed;
    }

    public void Rotate(int angle)
    {
        _currentAngle += angle;
    }

    public void Fire()
    {
        if (_rocketsCount > 0)
        {
            _rocketsCount--;
        }
    }
}