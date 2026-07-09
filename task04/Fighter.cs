namespace task04;

public class Fighter : ISpaceship
{
    private int _position = 0;
    private int _currentAngle = 0;

    private int _rocketsCount = 10;

    public int Speed => 100;
    public int FirePower => 50;

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