﻿using System.Drawing.Drawing2D;

namespace EditorModel
{
    /// <summary>
    /// Класс для наследования фигур
    /// </summary>
    public class Figure
    {
        /// <summary>
        /// Свойство трансформера фигуры
        /// </summary>
        public Matrix Transform { get; set; }

        /// <summary>
        /// Свойство источника геометрии фигуры
        /// </summary>
        public Geometry Geometry { get; set; }

        /// <summary>
        /// Свойство стиля рисования фигуры
        /// </summary>
        public Style Style { get; set; }

        /// <summary>
        /// Предоставление геометрии для рисования
        /// </summary>
        /// <returns></returns>
        public GraphicsPath GetTransformedPath()
        {
            return Geometry.Path;
        }

        /// <summary>
        /// Свойство рисовальщика фигуры
        /// </summary>
        public Renderer Renderer { get; set; }

    }
}