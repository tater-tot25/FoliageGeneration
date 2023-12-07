using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planePointCalculator : MonoBehaviour
{
    public Vector4 plane; //The entire Plane in math Form
    private bool set = false;
    //The four coords of our plane to chart the visual aid
    public Vector3 topLeft;
    [HideInInspector] public Vector3 topRight;
    [HideInInspector] public Vector3 bottomLeft;
    public Vector3 bottomRight;
    public float greenLineOffset; //bottomRight -> topRight (greenLine)
    public float magentaLineOffset; //bottomLeft -> bottomRight (magenta line)
    public float xTest;
    public float zTest;
    public bool isTestPointOnPlane;
    //Adjusts the shear amount for each point, can't go less than zero
    public float topLeftShearX;
    public float topRightShearX;
    public float bottomLeftShearX;
    public float bottomRightShearX;
    public float topLeftShearZ;
    public float topRightShearZ;
    public float bottomLeftShearZ;
    public float bottomRightShearZ;
    public float areaWithPoint;
    public float areaOfPlane;
    public bool reset;

    //given the cell's coords, return the full coord of where it should be spawned
    public Vector3 getPlaneCoordFrom2D(float xCell, float zCell)
    {
        float yVal = getYValue(xCell, zCell);
        Vector3 returnVal = new Vector3(xCell, yVal, zCell);
        return returnVal;
    }

    //given three points, calculate the area of a triangle

    public Vector3 getNormal()
    {
        Vector3 a = topLeft - topRight; //a - b
        Vector3 b = bottomLeft - topRight; //c - b
        Vector3 normal = Vector3.Cross(a, b);
        return normal;
    }


    private float getTriangleArea(Vector3 pointOne, Vector3 pointTwo, Vector3 pointThree)
    {
        float sideOne = System.Math.Abs((Vector3.Distance(pointOne, pointThree)));
        float sideTwo = System.Math.Abs((Vector3.Distance(pointTwo, pointThree)));
        float sideThree = System.Math.Abs((Vector3.Distance(pointTwo, pointOne)));
        float perim = (sideOne + sideTwo + sideThree) / 2;
        float area = (float)System.Math.Sqrt(perim * (perim-sideOne) * (perim-sideTwo) * (perim-sideThree));
        return area;
    }

    //given a point, determine if it lies within the shear bounds of the plane
    public bool isOnPlane(Vector3 point)
    {
        //Divide the area into a 4 trianges given the questioned point, and sum it
        float areaOne = System.Math.Abs((getTriangleArea(topLeft, topRight, point)));
        float areaTwo = System.Math.Abs((getTriangleArea(topLeft, bottomLeft, point)));
        float areaThree = System.Math.Abs(getTriangleArea(bottomLeft, bottomRight, point));
        float areaFour = System.Math.Abs(getTriangleArea(bottomRight, topRight, point));
        float sumArea = areaOne + areaTwo + areaThree + areaFour;
        areaWithPoint = sumArea;
        areaWithPoint = (float)System.Math.Round(areaWithPoint, 2);
        //Get the sum of two triangles dividing the plane and sum it
        float mainAreaOne = System.Math.Abs(getTriangleArea(topLeft, topRight, bottomLeft));
        float mainAreaTwo = System.Math.Abs(getTriangleArea(bottomLeft, bottomRight, topRight));
        float sumCompArea = mainAreaOne + mainAreaTwo;
        areaOfPlane = sumCompArea;
        areaOfPlane = (float)System.Math.Round(areaOfPlane, 2);
        //Compare the two areas and reaturn
        if(areaWithPoint == areaOfPlane)
        {
            return true;
        }
        return false;
    }


    //https://gamedev.stackexchange.com/questions/79736/find-point-in-3d-plane
    //Thanks to this for giving me an equation for calculating a point on a 3D plane
    public void setPlaneEquation(Vector3 pointOne, Vector3 pointTwo, Vector3 pointThree)
    {
        //This is literally black magick, I have no clue wtf this does, it creates the plane equation :(
        Vector3 tempOne = pointTwo - pointOne;
        Vector3 tempTwo = pointThree - pointTwo;
        Vector3 crossProduct = Vector3.Cross(tempOne, tempTwo);
        plane.x = crossProduct.x;
        plane.y = crossProduct.y;
        plane.z = crossProduct.z;
        plane.w = -(Vector3.Dot(crossProduct, pointOne));
        set = true;
    }

    //Get the point's y value, given the x and z coords
    //Must set the plane equation first
    public float getYValue(float x, float z)
    {
        return (((-plane.x)*x) - ((plane.z) * z) - plane.w) / plane.y;
    }


    //Force negate all the shear numbers
    public void ensureNegate()
    {
        generateFoliage temp = this.gameObject.GetComponent<generateFoliage>();
        int size = temp.pub_size;
        placeFoliage tempPlace = this.gameObject.GetComponent<placeFoliage>();
        float sizeFactor = tempPlace.seperationAmount;
        //X Shear Caps
        topLeftShearX = -(System.Math.Abs(topLeftShearX));
        if (topLeftShearX < -((size * sizeFactor) / 2))
        {
            topLeftShearX = -((size * sizeFactor) / 2);
        }
        topRightShearX = System.Math.Abs(topRightShearX);
        if (topRightShearX > ((size * sizeFactor) / 2))
        {
            topRightShearX = ((size * sizeFactor) / 2);
        }
        bottomLeftShearX = -(System.Math.Abs(bottomLeftShearX));
        if (bottomLeftShearX < -((size * sizeFactor) / 2))
        {
            bottomLeftShearX = -((size * sizeFactor) / 2);
        }
        bottomRightShearX = System.Math.Abs(bottomRightShearX);
        if (bottomRightShearX > ((size * sizeFactor) / 2))
        {
            bottomRightShearX = ((size * sizeFactor) / 2);
        }
        //Z Shear Caps
        topLeftShearZ = -System.Math.Abs(topLeftShearZ);
        if (topLeftShearZ < -((size * sizeFactor) / 2))
        {
            topLeftShearZ = -((size * sizeFactor) / 2);
        }
        topRightShearZ = -System.Math.Abs(topRightShearZ);
        if (topRightShearZ < -((size * sizeFactor) / 2))
        {
            topRightShearZ = -((size * sizeFactor) / 2);
        }
        bottomLeftShearZ = System.Math.Abs(bottomLeftShearZ);
        if (bottomLeftShearZ > ((size * sizeFactor) / 2))
        {
            bottomLeftShearZ = ((size * sizeFactor) / 2);
        }
        bottomRightShearZ = System.Math.Abs(bottomRightShearZ);
        if (bottomRightShearZ > ((size * sizeFactor) / 2))
        {
            bottomRightShearZ = ((size * sizeFactor) / 2);
        }
    }

    //Force the corners post-shear to be on the plane axis
    public void readjustShears()
    {
        float temp = getYValue(topLeft.x, topLeft.z);
        topLeft = new Vector3(topLeft.x, temp, topLeft.z);
        temp = getYValue(topRight.x, topRight.z);
        topRight = new Vector3(topRight.x, temp, topRight.z);
        temp = getYValue(bottomLeft.x, bottomLeft.z);
        bottomLeft = new Vector3(bottomLeft.x, temp, bottomLeft.z);
        temp = getYValue(bottomRight.x, bottomRight.z);
        bottomRight = new Vector3(bottomRight.x, temp, bottomRight.z);

    }

    private void OnDrawGizmos()
    {
        generateFoliage temp = this.gameObject.GetComponent<generateFoliage>();
        int size = temp.pub_size;
        placeFoliage tempPlace = this.gameObject.GetComponent<placeFoliage>();
        float sizeFactor = tempPlace.seperationAmount;
        ensureNegate();
        topLeft = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        topRight = new Vector3(this.transform.position.x + (size * sizeFactor), this.transform.position.y + greenLineOffset, this.transform.position.z);
        bottomLeft = new Vector3(this.transform.position.x, this.transform.position.y + magentaLineOffset, this.transform.position.z + (size * sizeFactor));
        bottomRight = new Vector3(this.transform.position.x + (size * sizeFactor), this.transform.position.y + greenLineOffset + magentaLineOffset, this.transform.position.z + (size * sizeFactor));
        setPlaneEquation(topLeft, topRight, bottomLeft);
        topLeft = new Vector3((topLeft.x) - topLeftShearX, topLeft.y, (topLeft.z) - topLeftShearZ);
        topRight = new Vector3((topRight.x) - topRightShearX, topRight.y, (topRight.z) - topRightShearZ);
        bottomLeft = new Vector3((bottomLeft.x) - bottomLeftShearX, bottomLeft.y, (bottomLeft.z) - bottomLeftShearZ);
        bottomRight = new Vector3((bottomRight.x) - bottomRightShearX, bottomRight.y, (bottomRight.z) - bottomRightShearZ);
        readjustShears();
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(bottomLeft, 0.1f);
        Gizmos.DrawSphere(bottomRight, 0.1f);
        Gizmos.DrawSphere(topLeft, 0.1f);
        Gizmos.DrawSphere(topRight, 0.1f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
        Gizmos.DrawLine(topLeft, bottomRight);
        Gizmos.DrawLine(bottomLeft, topRight);
        Vector3 tempTest = getPlaneCoordFrom2D(xTest, zTest);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(tempTest, 0.1f);
        isTestPointOnPlane = isOnPlane(tempTest);
        if (reset)
        {
            Vector3 v = this.transform.position;
            topLeft = v;
            topRight = new Vector3(v.x + size, v.y, v.z);
            bottomLeft = new Vector3(v.x, v.y, v.z + size);
            bottomRight = new Vector3(v.x + size, v.y, v.z + size);
            plane = new Vector4(2, 2, 2, 2);
            areaWithPoint = 0;
            areaOfPlane = 0;
            topLeftShearX = 0;
            topRightShearX = 0;
            bottomLeftShearX = 0;
            bottomRightShearX = 0;
            topLeftShearZ = 0;
            topRightShearZ = 0;
            bottomLeftShearZ = 0;
            bottomRightShearZ = 0;
            greenLineOffset = 0;
            magentaLineOffset = 0;
            xTest = this.transform.position.x;
            zTest = this.transform.position.z;
            setPlaneEquation(topLeft, topRight, bottomLeft);
            topLeft = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
            topRight = new Vector3(this.transform.position.x + (size * sizeFactor), this.transform.position.y + greenLineOffset, this.transform.position.z);
            bottomLeft = new Vector3(this.transform.position.x, this.transform.position.y + magentaLineOffset, this.transform.position.z + (size * sizeFactor));
            bottomRight = new Vector3(this.transform.position.x + (size * sizeFactor), this.transform.position.y + greenLineOffset + magentaLineOffset, this.transform.position.z + (size * sizeFactor));
            setPlaneEquation(topLeft, topRight, bottomLeft);
            topLeft = new Vector3((topLeft.x) - topLeftShearX, topLeft.y, (topLeft.z) - topLeftShearZ);
            topRight = new Vector3((topRight.x) - topRightShearX, topRight.y, (topRight.z) - topRightShearZ);
            bottomLeft = new Vector3((bottomLeft.x) - bottomLeftShearX, bottomLeft.y, (bottomLeft.z) - bottomLeftShearZ);
            bottomRight = new Vector3((bottomRight.x) - bottomRightShearX, bottomRight.y, (bottomRight.z) - bottomRightShearZ);
            readjustShears();
            reset = false;
        }
    }

    //First run
    private void Start()
    {
        generateFoliage temp = this.gameObject.GetComponent<generateFoliage>();
        int size = temp.pub_size;
        placeFoliage tempPlace = this.gameObject.GetComponent<placeFoliage>();
        float sizeFactor = tempPlace.seperationAmount;
        ensureNegate();
        topLeft = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z);
        topRight = new Vector3(this.transform.position.x + (size * sizeFactor), this.transform.position.y + greenLineOffset, this.transform.position.z);
        bottomLeft = new Vector3(this.transform.position.x, this.transform.position.y + magentaLineOffset, this.transform.position.z + (size * sizeFactor));
        bottomRight = new Vector3(this.transform.position.x + (size * sizeFactor), this.transform.position.y + greenLineOffset + magentaLineOffset, this.transform.position.z + (size * sizeFactor));
        setPlaneEquation(topLeft, topRight, bottomLeft);
        topLeft = new Vector3((topLeft.x) - topLeftShearX, topLeft.y, (topLeft.z) - topLeftShearZ);
        topRight = new Vector3((topRight.x) - topRightShearX, topRight.y, (topRight.z) - topRightShearZ);
        bottomLeft = new Vector3((bottomLeft.x) - bottomLeftShearX, bottomLeft.y, (bottomLeft.z) - bottomLeftShearZ);
        bottomRight = new Vector3((bottomRight.x) - bottomRightShearX, bottomRight.y, (bottomRight.z) - bottomRightShearZ);
        readjustShears();
    }
}
