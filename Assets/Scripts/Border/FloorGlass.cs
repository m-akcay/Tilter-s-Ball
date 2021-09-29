public class FloorGlass : BorderGlass
{
    protected override void Start()
    {
        base.Start();
        this.mat.SetFloat("thisWallPosX", 0.5f);
        this.mat.SetFloat("thisWallPosY", -1);
    }

}
