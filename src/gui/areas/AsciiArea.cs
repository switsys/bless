// created on 6/15/2004 at 4:10 PM
/*
 *   Copyright (c) 2004, Alexandros Frantzis (alf82 [at] freemail [dot] gr)
 *
 *   This file is part of Bless.
 *
 *   Bless is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 2 of the License, or
 *   (at your option) any later version.
 *
 *   Bless is distributed in the hope that it will be useful,
 *   but WITHOUT ANY WARRANTY; without even the implied warranty of
 *   MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *   GNU General Public License for more details.
 *
 *   You should have received a copy of the GNU General Public License
 *   along with Bless; if not, write to the Free Software
 *   Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */
using System;
using Gtk;
using Gdk;
using Bless.Gui.Drawers;

namespace Bless.Gui.Areas {

///<summary>An area that displays ascii</summary>
public class AsciiArea : Area {
	
	
	public AsciiArea()
	: base() 
	{
		type="ascii";
		dpb=1;
		canFocus=true;
	}

	protected override void RenderRowNormal(int i, int p, int n, bool blank)
	{
		int rx=0+x;
		int ry=i*drawer.Height+y;
		long roffset=offset+i*bpr+p;
		bool odd;
		Gdk.GC backEvenGC=drawer.GetBackgroundGC(Drawer.RowType.Even, Drawer.HighlightType.Normal);
		Gdk.GC backOddGC=drawer.GetBackgroundGC(Drawer.RowType.Odd, Drawer.HighlightType.Normal);
		
		
		// odd row?
		odd=(((roffset/bpr)%2)==1);
		
		if (blank==true) {
			if (odd)
				backPixmap.DrawRectangle(backOddGC, true, rx, ry, width, drawer.Height);
			else
				backPixmap.DrawRectangle(backEvenGC, true, rx, ry, width, drawer.Height);
		}
		
		Drawer.RowType rowType;
				
		if (odd)
			rowType=Drawer.RowType.Odd;
		else
			rowType=Drawer.RowType.Even;
			
		int pos=0;
		
		while (true) {
			
			if (pos>=p) { //don't draw until we reach p
				drawer.DrawNormal(backEvenGC, backPixmap, rx, ry, byteBuffer[roffset++], rowType, Drawer.ColumnType.Even);
				if (--n<=0)
					break;
			}
			
			rx=rx+drawer.Width;
			
			pos++;
		}
	}
	
	protected override void RenderRowHighlight(int i, int p, int n, bool blank, Drawer.HighlightType ht)
	{
		int rx=0+x;
		int ry=i*drawer.Height+y;
		long roffset=offset+i*bpr+p;
		bool odd;
		Gdk.GC backEvenGC=drawer.GetBackgroundGC(Drawer.RowType.Even, Drawer.HighlightType.Normal);
		Gdk.GC backOddGC=drawer.GetBackgroundGC(Drawer.RowType.Odd, Drawer.HighlightType.Normal);
		
		// odd row?
		odd=(((roffset/bpr)%2)==1);
		
		if (blank==true) {
			if (odd)
				backPixmap.DrawRectangle(backOddGC, true, rx, ry, width, drawer.Height);
			else
				backPixmap.DrawRectangle(backEvenGC, true, rx, ry, width, drawer.Height);
		}
		
		Drawer.RowType rowType;
				
		if (odd)
			rowType=Drawer.RowType.Odd;
		else
			rowType=Drawer.RowType.Even;
		
		int pos=0;
		
		while (true) {
			
			if (pos>=p) { //don't draw until we reach p
				drawer.DrawHighlight(backEvenGC, backPixmap, rx, ry, byteBuffer[roffset++], rowType, ht);
				if (--n<=0)
					break;
			}
			
			rx=rx+drawer.Width;
			
			pos++;
		}
	}
	
	public override int CalcWidth(int n, bool force) 
	{
		if (fixedBpr > 0 && n>fixedBpr && !force)
			return -1;
		else
			return n*drawer.Width;
	}

	public override void GetDisplayInfoByOffset(long off, out int orow, out int obyte, out int ox, out int oy)
	{	 
		orow=(int)((off-offset)/bpr);
		obyte=(int)((off-offset)%bpr);
			
		oy=orow*drawer.Height;
	
		ox=obyte*drawer.Width;
	} 

	public override long GetOffsetByDisplayInfo(int x, int y, out int digit, out GetOffsetFlags flags)
	{
		flags=0;
		int row=y/drawer.Height;
		int col=x/drawer.Width;
		long off=(row*bpr+col)+offset;
		
		if (off >= byteBuffer.Size)
			flags |= GetOffsetFlags.Eof;
		
		digit=0;
		
		return off;
	}
	
	public override bool HandleKey(Gdk.Key key, bool overwrite)
	{
		//System.Console.WriteLine("Ascii: {0}", key);
		
		if (key >= Gdk.Key.space && key <= Gdk.Key.asciitilde) {
			byte[] ba=new byte[]{(byte)key};
			
			if (cursorOffset==byteBuffer.Size)
				byteBuffer.Append(ba);	
			else if (overwrite==true)
				byteBuffer.Replace(cursorOffset, cursorOffset, ba);
			else if (overwrite==false)
				byteBuffer.Insert(cursorOffset, ba);
				
			return true;
		}
		else
			return false;
	}
}


}//namespace