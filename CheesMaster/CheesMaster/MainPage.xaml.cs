using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;

namespace CheesMaster
{
    public partial class MainPage : UserControl
    {
        //подключаем Window'ые библиотеки для имитации клика мыши
        [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
        public static extern void mouse_event(long dwFlags, long dx, long dy, long cButtons, long dwExtraInfo);

        //констаны для имитации мыши по сути нам только MOUSEEVENTF_LEFTUP нужен, остальные до кучи
        private const int MOUSEEVENTF_LEFTDOWN = 0x02;
        private const int MOUSEEVENTF_LEFTUP = 0x04;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x08;
        private const int MOUSEEVENTF_RIGHTUP = 0x10;

        //игровое поле
        private GameBoard gameBoard;

        //Флаг, что пользователь тащит фигуру
        private bool IsFigureDragging;

        //сюда запоминаем картинку фигуры, которую тащят
        private Image ImgOfDrugginFigure;

        //Список фигур в игре
        private List<Figure> FigureInGame;
        
        //Список битых фигуры
        private List<Figure> FigureBit;

        //координаты последней позиции при перетаскивании
        private double lastx;
        private double lasty;

        //клетка с которой сделали ход
        private Cell CellMadeMove;
        

        //конструктор страницы, он же точка входа, отсюда страница начнет работать
        public MainPage()
        {
            //создаются объекты указанные в страницы разметки
            InitializeComponent();

            //инициализируем флаг перетаскивания
            IsFigureDragging = false;

            //инициализируме игру
            InitGame();
        }

       
        /// Инициализируем игру
        private void InitGame()
        {
            //очищаем прошлые фигуры
            if (CanvasGameBoard.Children.Count > 0)
            {
                foreach (object obj in CanvasGameBoard.Children)
                {
                    if (obj is Image)
                        (obj as Image).Source = null;
                }
                CanvasGameBoard.Children.Clear();
            }

            //клетка которая сделал ход (в начале игры такой нет)
            CellMadeMove = null;

            //инициализируем новую доску
            gameBoard = new GameBoard();            
            

            //создаем списки фигур
            if (FigureInGame == null)
                FigureInGame = new List<Figure>();
            else
                FigureInGame.Clear();

            if (FigureBit == null)
                FigureBit = new List<Figure>();
            else
                FigureBit.Clear();

            //инициализируем клетки
            for (int i=0; i<=7; i++)
            {
                for (int j=0; j<=7; j++)
                {
                    gameBoard.Cells[i,j] = new Cell(i+1,j+1);
                    //добавляем клетку на холст
                    CanvasGameBoard.Children.Add(gameBoard.Cells[i,j].rectangle);
                    //подписываем клетку на событие отпускание мыши при перетаскивании фигуры
                    gameBoard.Cells[i, j].rectangle.MouseLeftButtonUp += new MouseButtonEventHandler(rectangle_MouseLeftButtonUp);
                }
            }

            //инициализируем фигуры (делаем это отдельно иначе фигуры залазят под клетки)
            for (int i=0; i<=7; i++)
            {
                for (int j=0; j<=7; j++)
                {
                  switch (i)
                        {
                            //черные фигуры
                            case 0:
                                switch (j)
                                { 
                                        case 0 : gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, true, 2);
                                            break;
                                        case 1: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, true, 3);
                                            break;
                                        case 2: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, true, 4);
                                            break;
                                        case 3: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, true, 6);
                                            break;
                                        case 4: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, true, 5);
                                            break;
                                        case 5: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, true, 4);
                                            break;
                                        case 6: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, true, 3);
                                            break;
                                        case 7: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, true, 2);
                                            break;
                                        default: break;
                                }
                                break;

                            //черные пешки
                            case 1:
                                gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, true, 1);
                                break;
                            //белые пешки
                            case 6:
                                gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, false, 1);
                                break;
                            //белые фигуры
                            case 7:
                                switch (j)
                                { 
                                        case 0 : gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, false, 2);
                                            break;
                                        case 1: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, false, 3);
                                            break;
                                        case 2: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, false, 4);
                                            break;
                                        case 3: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, false, 6);
                                            break;
                                        case 4: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, false, 5);
                                            break;
                                        case 5: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, false, 4);
                                            break;
                                        case 6: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, false, 3);
                                            break;
                                        case 7: gameBoard.Cells[i, j].InitFigure(i + 1, j + 1, false, 2);
                                            break;
                                        default: break;
                                }
                                break;
                            default: break;
                        }
                        if ((gameBoard.Cells[i, j].figure != null) && (gameBoard.Cells[i, j].figure.img != null))
                        {
                            //добавляем фигуру в список фигур в игре
                            FigureInGame.Add(gameBoard.Cells[i, j].figure);
                            //добавляем фигуры на холст
                            CanvasGameBoard.Children.Add(gameBoard.Cells[i, j].figure.img);

                            //подписываем картинку фигуры на события мыши
                            gameBoard.Cells[i, j].figure.img.MouseLeftButtonDown +=new MouseButtonEventHandler(img_MouseLeftButtonDown);
                            gameBoard.Cells[i, j].figure.img.MouseLeftButtonUp +=new MouseButtonEventHandler(img_MouseLeftButtonUp);
                            gameBoard.Cells[i, j].figure.img.MouseMove +=new MouseEventHandler(img_MouseMove);
                        }
                  }
              }
        }

        //начинаем тащить фигуру
        private void img_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Image img = sender as Image;

            if (img != null)
            {
                //определяем клетку
                CellMadeMove = gameBoard.GetCellByImg(img);

                if (CellMadeMove != null)
                {
                    ImgOfDrugginFigure = img;
                    if (CellMadeMove.GetCellCollor())
                        CellMadeMove.rectangle.Fill = new SolidColorBrush(Colors.Orange);
                    else
                        CellMadeMove.rectangle.Fill = new SolidColorBrush(Colors.Yellow);
                }
                else
                {
                    return;
                }
            }
            else
                return;

            // Запоминаем текущее положение мыши
            lastx = e.GetPosition(null).X;
            lasty = e.GetPosition(null).Y;            

            // Начинаем тащить, "хватаем" картинку, что бы можно было её указывать координаты явно
            ((FrameworkElement)sender).CaptureMouse();
            //стивим флаг, что идет перетаскивание
            IsFigureDragging = true;
        }

        //тащим фигуру
        private void img_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsFigureDragging)
            {
                double x = e.GetPosition(null).X;
                double y = e.GetPosition(null).Y;

                double dx = x - lastx;
                double dy = y - lasty;

                lastx = x;
                lasty = y;

                //устанавливаем картинке фигуры новыее координаты
                ((FrameworkElement)sender).SetValue(Canvas.LeftProperty, (double)((FrameworkElement)sender).GetValue(Canvas.LeftProperty) + dx);
                ((FrameworkElement)sender).SetValue(Canvas.TopProperty, (double)((FrameworkElement)sender).GetValue(Canvas.TopProperty) + dy);
            }
        }


        //ловим клетку над которой отпустили фигуру
        //private void rectangle_Drop(object sender, DragEventArgs e)
        private void rectangle_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (IsFigureDragging&&(sender!=null)&&(CellMadeMove!=null)&&(sender is Rectangle))
            {
                Rectangle rec = sender as Rectangle;
                Cell cell = gameBoard.GetCellByRectangle(rec);
                cell.figure = CellMadeMove.figure;
                cell.figure.SetFigureToCell(cell.rown, cell.coln);
                CellMadeMove.SetCellDefaultcolor();
                CellMadeMove.figure = null;
                CellMadeMove = null;
                cell.figure.img = ImgOfDrugginFigure;
                IsFigureDragging = false;

            }            
        }

        //заканчиваем тащить фигуру
        private void img_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            //отпускаем картинку, что бы её можно было поставить в клетку
            ((FrameworkElement)sender).ReleaseMouseCapture();
            //т.к. мы перехватили отпускание левой клавиши мыши, то rectangle_MouseLeftButtonUp
            //не вызовится, значит нужно повторно сымитировать клик мыши
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);                
        }

        //кнопка начать игру
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            InitGame();
        }

    }

    //Класс игровая доска
    public class GameBoard
    {
        //массив клеток
        public Cell[,] Cells = new Cell[8, 8];
        public GameBoard()
        { 
        }

        //покане работает корректно на разных типах экранов
        /*
        public Cell GetCellByXY( double x, double y)
        {
           
            int j = Convert.ToInt32(Math.Floor((x -20) / 80)-5);
            int i = Convert.ToInt32(Math.Floor((y -20) / 80)-1);
            if (Cells[i, j] != null)
            {
                return Cells[i, j];
            }
            else
                return null;
            
        }
    */

        //определяем клетку по картинке
        public Cell GetCellByImg(Image img)
        {
            foreach (Cell cell in Cells)
            { 
                if ((cell.figure !=null )&&(cell.figure.img != null))
                {
                    if (cell.figure.img==img)
                        return cell;
                }
            }
            return null;
        }

        //определяем клетку по прямоугольнику
        public Cell GetCellByRectangle(Rectangle rec)
        {
            foreach (Cell cell in Cells)
            {
                if (cell.rectangle != null) 
                {
                    if (cell.rectangle == rec)
                        return cell;
                }
            }
            return null;
        }
    }

    //Класс игровая ячейка - "клетка"
    public class Cell
    {
        //холстдля закраски в черное или белое
        //public Canvas canv;
        //фигура
        public Figure figure;
        //номер строки
        public int rown;
        //номер колонки
        public int coln;
        //отрисовка клетки
        public Rectangle rectangle; 

        //конструктор
        public Cell(int r, int c)
        {
            rown = r;
            coln = c;

            rectangle = new Rectangle();
            SetCellDefaultcolor();
            /*
            if (GetCellCollor(r, c))
                rectangle.Fill = new SolidColorBrush(Colors.Black);
            else
                rectangle.Fill = new SolidColorBrush(Colors.White);
             */
            Canvas.SetLeft(rectangle, (c-1)*80);
            Canvas.SetTop(rectangle, (r-1)*80);
            rectangle.Width = 80;
            rectangle.Height = 80;
            /*
                canv = new Canvas();
                //привязываем холст к ячейки грида
                canv.SetValue(Grid.RowProperty, r);
                canv.SetValue(Grid.ColumnProperty, c);
                canv.Width = 80;
                canv.Height = 80;
                canv.AllowDrop = true;
                //закрашиваем в белое или черное
                if (GetCellCollor(r, c))
                    canv.Background = new SolidColorBrush(Colors.Black);
                else
                    canv.Background = new SolidColorBrush(Colors.White);
            */


        }

        public void SetCellDefaultcolor()
        {
            if (GetCellCollor())
                rectangle.Fill = new SolidColorBrush(Colors.Black);
            else
                rectangle.Fill = new SolidColorBrush(Colors.White);
        }

        //создаем фигуру в клетке
        public void InitFigure(int r, int c, bool IsBlack, int fnum)
        {
            figure = new Figure(r, c, IsBlack, fnum, this);
            //canv.Children.Add(figure.img);
            SetFigureToCellSize();
        }

        //устанавливаем размер картинки по размеру холста
        public void SetFigureToCellSize()
        {
            figure.img.Width = 80;
            figure.img.Height = 80;
        }

        //получаем цвет клетки
        public bool GetCellCollor()
        {
            if ((rown % 2 == 1) && (coln % 2 == 1))
                return false;
            if ((rown % 2 == 0) && (coln % 2 == 0))
                return false;
            else
                return true;
        }
    }

    //класс фигура
    public class Figure
    {
        /*
         * 1 - Пешка
         * 2 - Ладья
         * 3 - Конь
         * 4 - Слон
         * 5 - Ферзь
         * 6 - Король
         */
        //картинка с фигурой
        public Image img;
        //цвет
        public bool isBlack;
        //клетка на которой стоит фигура
        public Cell cell;

        public Figure(int r, int c, bool IsBlack, int fnum, Cell _cell)
        { 
            img = new Image();
            isBlack = IsBlack;

            if (_cell != null)
                cell = _cell;

            //привязываем картинку к фигуре
            if (isBlack)
            {               
                switch (fnum)
                {

                    case 1: img.Source = new BitmapImage(new Uri("/CheesMaster;component/ImagesFigure/bpawn.png", UriKind.Relative)); 
                        break;
                    case 2: img.Source = new BitmapImage(new Uri("/CheesMaster;component/ImagesFigure/btower.png", UriKind.Relative)); 
                        break;
                    case 4: img.Source = new BitmapImage(new Uri("/CheesMaster;component/ImagesFigure/belephant.png", UriKind.Relative)); 
                        break;
                    case 3: img.Source = new BitmapImage(new Uri("/CheesMaster;component/ImagesFigure/bhorse.png", UriKind.Relative)); 
                        break;
                    case 5: img.Source = new BitmapImage(new Uri("/CheesMaster;component/ImagesFigure/bqueen.png", UriKind.Relative)); 
                        break;
                    case 6: img.Source = new BitmapImage(new Uri("/CheesMaster;component/ImagesFigure/bking.png", UriKind.Relative)); 
                        break;
                    default: break;
                }
            }
            else 
            {
                switch (fnum)
                {
                    case 1: img.Source = new BitmapImage(new Uri("/CheesMaster;component/ImagesFigure/wpawn.png", UriKind.Relative));                         
                        break;
                    case 2: img.Source = new BitmapImage(new Uri("/CheesMaster;component/ImagesFigure/wtower.png", UriKind.Relative));                         
                        break;
                    case 4: img.Source = new BitmapImage(new Uri("/CheesMaster;component/ImagesFigure/welephant.png", UriKind.Relative));                         
                        break;
                    case 3: img.Source = new BitmapImage(new Uri("/CheesMaster;component/ImagesFigure/whorse.png", UriKind.Relative));                                                 
                        break;
                    case 5: img.Source = new BitmapImage(new Uri("/CheesMaster;component/ImagesFigure/wqueen.png", UriKind.Relative));                         
                        break;
                    case 6: img.Source = new BitmapImage(new Uri("/CheesMaster;component/ImagesFigure/wking.png", UriKind.Relative));                         
                        break;
                    default: break;
                }            
            }
            SetFigureToCell(r, c);
            //img.SetValue(Grid.RowProperty, r);
            //img.SetValue(Grid.ColumnProperty, c);
              
        }

        //устанавливаем фигуру в клетку
        public void SetFigureToCell(int r, int c)
        {
            Canvas.SetLeft(img, (c - 1) * 80);
            Canvas.SetTop(img, (r - 1) * 80);
        }
    }
}
