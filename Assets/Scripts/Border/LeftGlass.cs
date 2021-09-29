public class LeftGlass : BorderGlass
{
    protected override void Start()
    {
        base.Start();
        this.mat.SetFloat("thisWallPosX", 0);
        this.mat.SetFloat("thisWallPosY", -0.5f);
    }

}
