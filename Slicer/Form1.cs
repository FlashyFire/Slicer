using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Slicer.ObjectModel;
using Slicer.Import;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Slicer
{
    public partial class Form1 : Form
    {
        private readonly Model model = new Model();
        private bool isLoaded = false;
        private float scale = 1f;
        private Vector3 position = Vector3.Zero;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void FileExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void FileOpen_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                STLImporter importer = new STLImporter(model);
                try
                {
                    importer.Import(openFileDialog1.FileName);
                    SetupScaleAndPosition();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не могу открыть файл!\n{openFileDialog1.FileName}\n{ex.Message}", "Ошибка!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    model.Clear();
                    scale = 1f;
                    position = Vector3.Zero;
                }
                glControl1.Invalidate();
            }
        }

        private void glControl1_Load(object sender, EventArgs e)
        {
            isLoaded = true;
            SetupViewPort();
        }

        private void glControl1_Resize(object sender, EventArgs e)
        {
            if (!isLoaded)
            {
                return;
            }
            SetupViewPort();
            glControl1.Invalidate();
        }

        private void glControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!isLoaded)
            {
                return;
            }

            GL.ClearColor(Color.White);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();

            SetupCamera();
            SetupLight();

            GL.Enable(EnableCap.Lighting);
            GL.Enable(EnableCap.DepthTest);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            GL.ShadeModel(ShadingModel.Smooth);
            SetupMaterial(Color4.Gray, Color4.White, 100f);
            DrawFloor(0, 0, 0, 7.5, 10);

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            GL.ShadeModel(ShadingModel.Flat);
            SetupMaterial(Color4.CornflowerBlue, Color4.White, 100f);
            DrawModel();

            glControl1.SwapBuffers();
        }

        private void SetupViewPort()
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            double factor = glControl1.Width * 1.0 / glControl1.Height;
            float fov = (float)(45.0 * Math.PI / 180.0);
            Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(fov, (float)factor, 0.1f, 100f);
            GL.LoadMatrix(ref perspective);
            GL.Viewport(glControl1.ClientRectangle);
        }

        private void SetupCamera()
        {
            double alfa = (double)45 * Math.PI / 180.0;
            double psi = (double)40 * Math.PI / 180.0;

            double z = (double)20 * Math.Sin(alfa);
            double r = (double)20 * Math.Cos(alfa);
            float x = (float)(r * Math.Cos(psi));
            float y = (float)(-r * Math.Sin(psi));

            Matrix4 camera = Matrix4.LookAt(new Vector3(x, y, (float)z), new Vector3(0f, 0f, 0f), new Vector3(-x, -y, 0f));
            GL.LoadMatrix(ref camera);
        }

        private void SetupLight()
        {
            GL.Disable(EnableCap.Light0);

            GL.Light(LightName.Light0, LightParameter.Ambient, new float[] { 0.3f, 0.3f, 0.3f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Diffuse, new float[] { 0.5f, 0.5f, 0.5f, 1.0f });
            GL.Light(LightName.Light0, LightParameter.Specular, new float[] { 0.05f, 0.05f, 0.05f, 1.0f });
            GL.LightModel(LightModelParameter.LightModelAmbient, new float[] { 0.4f, 0.4f, 0.4f, 1.0f });
            GL.LightModel(LightModelParameter.LightModelColorControl, 0x81fa);
            GL.LightModel(LightModelParameter.LightModelLocalViewer, 1f);
            GL.LightModel(LightModelParameter.LightModelTwoSide, 0f);

            SetLightPosition();

            //GL.Light(LightName.Light0, LightParameter.SpotCutoff, 180f);
            //GL.Light(LightName.Light0, LightParameter.SpotExponent, 0f);

            GL.Enable(EnableCap.Light0);
        }

        /// <summary>
        /// Задать позицию источника света
        /// </summary>
        private void SetLightPosition()
        {
            double alfa = (double)40 * Math.PI / 180.0;
            Matrix4 matrix;
            Matrix4.CreateRotationZ((float)-alfa, out matrix);

            Vector4 lightPos = new Vector4(0f, 0f, 10f, 1f);
            Vector4.Transform(ref lightPos, ref matrix, out lightPos);
            GL.Light(LightName.Light0, LightParameter.Position, lightPos);

            Vector4 spotDir = new Vector4(0f, 0f, -1f, 1f);
            Vector4.Transform(ref spotDir, ref matrix, out spotDir);
            GL.Light(LightName.Light0, LightParameter.SpotDirection, spotDir);
        }

        void SetupMaterial(Color4 ambientAndDiffuse, Color4 specular, float shininess)
        {
            GL.Material(MaterialFace.Front, MaterialParameter.AmbientAndDiffuse, ambientAndDiffuse);
            GL.Material(MaterialFace.Front, MaterialParameter.Specular, specular);
            GL.Material(MaterialFace.Front, MaterialParameter.Emission, Color4.Black);
            GL.Material(MaterialFace.Front, MaterialParameter.Shininess, shininess);
        }

        void DrawFloor(double cx, double cy, double cz, double size, int n)
        {
            GL.PushMatrix();
            GL.Translate(cx, cy, cz);
            GL.Begin(PrimitiveType.Quads);

            double step = 2 * size / n;
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    GL.Vertex3(i * step - size, j * step - size, 0.0);
                    GL.Vertex3((i + 1) * step - size, j * step - size, 0.0);
                    GL.Vertex3((i + 1) * step - size, (j + 1) * step - size, 0.0);
                    GL.Vertex3(i * step - size, (j + 1) * step - size, 0.0);
                }
            }

            GL.End();
            GL.PopMatrix();
        }

        private void DrawModel()
        {
            foreach (Facet facet in model.Facets)
            {
                GL.Begin(PrimitiveType.Triangles);
                GL.Normal3(facet.Normal);
                GL.Vertex3(facet.Vertex1 * scale + position);
                GL.Vertex3(facet.Vertex2 * scale + position);
                GL.Vertex3(facet.Vertex3 * scale + position);
                GL.End();
            }
        }

        private void SetupScaleAndPosition()
        {
            ModelDimensions dimensions = model.Dimensions;
            scale = 0f;
            float dx = dimensions.Max.X - dimensions.Min.X;
            float dy = dimensions.Max.Y - dimensions.Min.Y;
            float dz = dimensions.Max.Z - dimensions.Min.Z;
            float size = (float)(75 / 10);
            CalcScale(dx, size);
            CalcScale(dy, size);
            CalcScale(dz, size);
            if (scale == 0f)
                scale = 1f;
            // Расчитать позицию (расположить модель посередине стола)
            position.X = -(dx / 2 + dimensions.Min.X) * scale;
            position.Y = -(dy / 2 + dimensions.Min.Y) * scale;
            position.Z = (-dimensions.Min.Z) * scale;
        }

        private void CalcScale(float size, float maxSize)
        {
            if (size == 0f)
                return;
            if (scale == 0f)
                scale = maxSize / size;
            else
                scale = Math.Min(scale, maxSize / size);
        }
    }
}
