public class RightGlass : BorderGlass
{
    protected override void Start()
    {
        base.Start();
        this.mat.SetFloat("thisWallPosX", 1);
        this.mat.SetFloat("thisWallPosY", -0.5f);
    }

}
