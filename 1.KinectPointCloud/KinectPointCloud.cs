using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.Sensor;
using System.Threading.Tasks;

public class KinectPointCloud : MonoBehaviour
{
    // Start is called before the first frame update
    Device kinect;
    int num;
    Mesh mesh;
    Vector3[] vertices;
    Color32[] colors;
    int[] indices;
    Transformation transformation;

    int width;
    int height;

    void Start()
    {
        InitKinect();
        InitMesh();

        Task t = kinectLoop();
    }

    private void InitKinect()
    {
        kinect = Device.Open(0);
        kinect.StartCameras(new DeviceConfiguration
        {
            ColorFormat = ImageFormat.ColorBGRA32,
            ColorResolution = ColorResolution.R720p,
            DepthMode = DepthMode.NFOV_Unbinned,
            SynchronizedImagesOnly = true,
            CameraFPS = FPS.FPS30
        });
        transformation = kinect.GetCalibration().CreateTransformation();
    }

    private void InitMesh()
    {
        width = kinect.GetCalibration().DepthCameraCalibration.ResolutionWidth;
        height = kinect.GetCalibration().DepthCameraCalibration.ResolutionHeight;
        num = width * height;

        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;

        vertices = new Vector3[num];
        colors = new Color32[num];
        indices = new int[num];

        for (int i = 0; i < num; i++)
        {
            indices[i] = i;
        }

        mesh.vertices = vertices;
        mesh.colors32 = colors;
        mesh.SetIndices(indices, MeshTopology.Points, 0);

        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }

    private async Task kinectLoop()
    {
        while (true)
        {
            using (Capture capture = await Task.Run(() => kinect.GetCapture()).ConfigureAwait(true))
            {
                //Getting color information
                Image colorImage = transformation.ColorImageToDepthCamera(capture);
                BGRA[] colorArray = colorImage.GetPixels<BGRA>().ToArray();

                //Getting vertices of point cloud
                Image xyzImage = transformation.DepthImageToPointCloud(capture.Depth);
                Short3[] xyzArray = xyzImage.GetPixels<Short3>().ToArray();

                for (int i = 0; i < num; i++)
                {
                    vertices[i].x = xyzArray[i].X * 0.001f;
                    vertices[i].y = -xyzArray[i].Y * 0.001f;
                    vertices[i].z = xyzArray[i].Z * 0.001f;

                    colors[i].b = colorArray[i].B;
                    colors[i].g = colorArray[i].G;
                    colors[i].r = colorArray[i].R;
                    colors[i].a = 255;
                }

                mesh.vertices = vertices;
                mesh.colors32 = colors;
                mesh.RecalculateBounds();
            }
        }
    }

    private void OnDestroy()
    {
        kinect.StopCameras();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
