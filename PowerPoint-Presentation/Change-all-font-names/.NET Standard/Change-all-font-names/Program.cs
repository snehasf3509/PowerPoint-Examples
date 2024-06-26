﻿//Load or open an PowerPoint Presentation
using Syncfusion.Presentation;

using (FileStream inputStream = new(Path.GetFullPath(@"../../../Template.pptx"), FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
{
    //Open an existing PowerPoint presentation.
    using (IPresentation pptxDoc = Presentation.Open(inputStream))
    {
        //Replace font of the all text contents in the presentation.
        ReplaceAllFonts(pptxDoc, "Times New Roman");
        //Save the presentation
        using (FileStream outputStream = new FileStream(@"../../../Result.pptx", FileMode.Create, FileAccess.Write)){
            pptxDoc.Save(outputStream);
        }
    }
}


/// <summary>
/// Iterate through each slide in the Presentation.
/// </summary>
void ReplaceAllFonts(IPresentation pptxDoc, string replacingFontName)
{
    //Iterate in master slides.
    foreach (IMasterSlide masterSlide in pptxDoc.Masters)
    {
        foreach (ILayoutSlide layoutSlide in masterSlide.LayoutSlides)
        {
            //Iterate in layout slide.
            IterateShapes(layoutSlide.Shapes, replacingFontName);
        }
        IterateShapes(masterSlide.Shapes, replacingFontName);
    }
    //Iterate in slides.
    foreach (ISlide slide in pptxDoc.Slides)
    {
        //Iterate in layout slide.
        IterateShapes(slide.LayoutSlide.Shapes, replacingFontName);
        IterateShapes(slide.Shapes, replacingFontName);
    }
}

/// <summary>
/// Iterate through each shapes in the slide.
/// </summary>
void IterateShapes(IShapes shapes, string replacingFontName)
{
    //Iterate through each shape in the slide.
    foreach (IShape shape in shapes)
    {
        //Identify the slide item type of the shape.
        switch (shape.SlideItemType)
        {
            case SlideItemType.AutoShape:
                //Replace font of the text content in textbody of the shape.
                ReplaceSpecificFontInTextPart(shape.TextBody, replacingFontName);
                break;
            //Identify the table from the shape collection.
            case SlideItemType.Table:
                ITable table = (ITable)shape;
                //Iterate through each row from the table.
                foreach (IRow row in table.Rows)
                {
                    //Iterate through each cell in the row.
                    foreach (ICell cell in row.Cells)
                    {
                        //Replace font of the text content in the textbody of the cell.
                        ReplaceSpecificFontInTextPart(cell.TextBody, replacingFontName);
                    }
                }
                break;
            case SlideItemType.Placeholder:
                ReplaceSpecificFontInTextPart(shape.TextBody, replacingFontName);
                break;
            case SlideItemType.GroupShape:
                IGroupShape groupShape = (IGroupShape)shape;
                //Iterate the shape collection in a group shape.
                foreach (IShape childShape in groupShape.Shapes)
                {
                    //Replace font of the text content in the textbody of the child shapes.
                    ReplaceSpecificFontInTextPart(childShape.TextBody, replacingFontName);
                }
                break;
        }
    }
}

/// <summary>
/// Iterate through each textpart of the paragraph and set the font name.
/// </summary>
void ReplaceSpecificFontInTextPart(ITextBody textBody, string replacingFontName)
{
    if (!string.IsNullOrEmpty(textBody.Text))
    {
        //Iterate through each paragraph of the textbody.
        foreach (IParagraph paragraph in textBody.Paragraphs)
        {
            paragraph.EndParagraphFont.FontName = replacingFontName;
            //Iterate through each textpart of the paragraph.
            foreach (ITextPart textPart in paragraph.TextParts)
            {
                //Replace the font name.
                    textPart.Font.FontName = replacingFontName;
            }
        }
    }
}