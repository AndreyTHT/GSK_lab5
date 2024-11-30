using System;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace GSK_lab5
{
    public partial class Form1 : Form
    {
        // GLControl для OpenGL отображения
        private GLControl glControl1;

        // Переменные для движения шарика
        float dx = 0, dy = 0; // Смещения по X и Y
        float sx = 0.004f, sy = 0.0033f; // Скорость движения
        float directX = -1; // Направление движения по X
        float directY = -1; // Направление движения по Y

        public Form1()
        {
            InitializeComponent();

            // Создание и настройка GLControl
            glControl1 = new GLControl();
            glControl1.Resize += GLControl_Resize; // Обработчик изменения размеров окна
            glControl1.Load += GLControl_Load;     // Обработчик загрузки
            glControl1.Paint += GLControl_Paint;   // Обработчик отрисовки
            glControl1.Dock = DockStyle.Fill;      // Заполняет весь контейнер
            pictureBox1.Controls.Add(glControl1);  // Добавление на форму
        }

        // Настройка области просмотра (Viewport) при изменении размеров окна
        private void GLControl_Resize(object sender, EventArgs e)
        {
            GL.Viewport(0, 0, glControl1.Width, glControl1.Height); // Настройка области видимости OpenGL
        }

        // Инициализация OpenGL при загрузке GLControl
        private void GLControl_Load(object sender, EventArgs e)
        {
            GL.ClearColor(1.0f, 1.0f, 0.5f, 1.0f); // Установка фонового цвета (жёлтый)
        }

        // Основной метод рисования
        private void GLControl_Paint(object sender, PaintEventArgs e)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit); // Очистка экрана и буфера глубины

            GL.MatrixMode(MatrixMode.Projection); // Установка матрицы проекции
            GL.LoadIdentity(); // Единичная матрица для сброса предыдущих преобразований

            // Настройка ортогональной проекции
            float ratio = (float)(glControl1.Width / (float)glControl1.Height);
            if (ratio > 1)
                GL.Ortho(0.0, 30.0 * ratio, 0.0, 30.0, -10, 10); // Расширяем видимую область по ширине
            else
                GL.Ortho(0.0, 30.0 / ratio, 0.0, 30.0, -10, 10); // Расширяем видимую область по высоте

            // Рисование красного квадрата
            GL.Color3(1f, 0, 0); // Установка красного цвета
            GL.Begin(PrimitiveType.Quads); // Начало рисования четырёхугольника
            GL.Vertex2(11.5f, 11.5f); // Левый нижний угол
            GL.Vertex2(11.5f, 18.5f); // Левый верхний угол
            GL.Vertex2(18.5f, 18.5f); // Правый верхний угол
            GL.Vertex2(18.5f, 11.5f); // Правый нижний угол
            GL.End();

            // Рисование шестиугольника
            GL.Color3(0.7f, 0.5f, 0.8f); // Установка цвета шестиугольника
            GL.Begin(PrimitiveType.LineLoop); // Замкнутая линия
            float hexRadius = 10; // Радиус шестиугольника
            for (int i = 0; i < 6; i++) // Цикл для каждой из 6 вершин
            {
                double angle = Math.PI / 3 * i; // Угол вершины (шаг 60 градусов)
                float x0 = 15 + (float)(hexRadius * Math.Cos(angle)); // Координата X вершины
                float y0 = 15 + (float)(hexRadius * Math.Sin(angle)); // Координата Y вершины
                GL.Vertex2(x0, y0); // Установка вершины
            }
            GL.End();

            // Рисование границы сцены
            GL.Color3(1f, 0, 0); // Красный цвет
            GL.Begin(PrimitiveType.LineLoop); // Замкнутый контур
            GL.Vertex2(1, 1); GL.Vertex2(1, 29); // Левая граница
            GL.Vertex2(29, 29); GL.Vertex2(29, 1); // Верхняя и нижняя границы
            GL.End();

            // Движущийся шарик
            GL.MatrixMode(MatrixMode.Modelview); // Установка матрицы модели
            GL.PushMatrix(); // Сохранение текущей матрицы
            GL.LoadIdentity(); // Сброс матрицы
            GL.Translate(dx, dy, 0); // Перемещение по X и Y

            GL.Begin(PrimitiveType.TriangleFan); // Веер треугольников для круга
            GL.Color3(0.4f, 0.5f, 1.0f); // Цвет центра
            double xc = 15, yc = 15, r = 1; // Центр и радиус шарика
            GL.Vertex2(xc, yc); // Центр круга
            for (int i = 0; i <= 30; i++) // Вершины круга (30 сегментов)
            {
                double x = xc + r * Math.Sin(i * Math.PI / 15); // X координата вершины
                double y = yc + r * Math.Cos(i * Math.PI / 15); // Y координата вершины
                GL.Vertex2(x, y); // Установка вершины
            }
            GL.End();

            // Логика движения
            if (dx <= -13) directX = 1; // Изменение направления при достижении границы
            if (dx > 13) directX = -1;
            dx += directX * sx; // Обновление смещения по X
            if (dy <= -13) directY = 1;
            if (dy > 13) directY = -1;
            dy += directY * sy; // Обновление смещения по Y

            GL.PopMatrix(); // Восстановление матрицы
            glControl1.SwapBuffers(); // Обмен буферов для отображения сцены
            glControl1.Invalidate(); // Принудительная перерисовка
        }
    }
}
