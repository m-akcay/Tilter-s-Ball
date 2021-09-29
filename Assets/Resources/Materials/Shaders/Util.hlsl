bool isCloseToBorder(in half worldPosY, out half distanceToBorder)
{
    half distY = distance(worldPosY, 0);
    if (distY < 0.5f)
    {
        distanceToBorder = distY;
        return true;
    }
    else if (distY > 0.5f)
    {
        distanceToBorder = 1 - distY;
        return true;
    }

    return false;
}

bool isCloseToBorder_ceiling(in half worldPosX, out half distanceToBorder)
{
    half distX = distance(worldPosX, 0);
    if (distX < 0.5)
    {
        distanceToBorder = distX;
        return true;
    }
    else if (distX > 0.5)
    {
        distanceToBorder = 1 - distX;
        return true;
    }

    return false;
}


bool ballWallDist(in half3 ballPos, in half3 worldPos, out half lerpValue)
{
    half radius = 0.2f;
    half distX = distance(ballPos.x, worldPos.x);
    if (distX < 0.5f)
    {
        half distY = distance(ballPos.y, worldPos.y);
        half distZ = distance(ballPos.z, worldPos.z);

        // squared
        half distToCenter = (distY * distY + distZ * distZ);
        if (distToCenter > 0.09f)
            return false;

        lerpValue = 1 - smoothstep(0, clamp(radius - distX / 2, 0, 1), distToCenter * 2);
        //alpha += 1 - smoothstep(0, distX,)
        return true;
    }

    return false;
}

bool ballWallDist_ceiling(in half3 ballPos, in half3 worldPos, out half lerpValue)
{
    half radius = 0.2f;
    half distY = distance(ballPos.y, worldPos.y);
    if (distY < 0.5f)
    {
        half distX = distance(ballPos.x, worldPos.x);
        half distZ = distance(ballPos.z, worldPos.z);

        // squared
        half distToCenter = (distX * distX + distZ * distZ);
        if (distToCenter > 0.09f)
            return false;

        lerpValue = 1 - smoothstep(0, clamp(radius - distY / 2, 0, 1), distToCenter * 2);
        //alpha += 1 - smoothstep(0, distX,)
        return true;
    }

    return false;
}

