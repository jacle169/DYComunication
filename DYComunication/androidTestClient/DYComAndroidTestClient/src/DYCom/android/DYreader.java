package DYCom.android;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;

import org.apache.http.util.EncodingUtils;

public class DYreader {
	ByteBuffer buf = null;

	public DYreader(byte[] data) {
		buf = ByteBuffer.wrap(data);
		buf.order(ByteOrder.LITTLE_ENDIAN);
	}

	public double Readdouble = 0;

	public boolean ReadDouble() {
		try {
			Readdouble = buf.getDouble();
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	public long Readint64 = 0;

	public boolean ReadInt64() {
		try {
			Readint64 = buf.getLong();
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	public short Readint16 = 0;

	public boolean ReadInt16() {
		try {
			Readint16 = buf.getShort();
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	public byte Readbyte = 0;

	public boolean ReadByte() {
		try {
			Readbyte = buf.get();
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	public String Readstring = "";

	public boolean ReadString(String charter) {
		try {
			int strlen = buf.getInt();
			byte[] strbytes = new byte[strlen];
			buf.get(strbytes);
			Readstring = EncodingUtils.getString(strbytes, charter);
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	public int Readint32 = 0;

	public boolean ReadInt32() {
		try {
			Readint32 = buf.getInt();
			return true;
		} catch (Exception e) {
			return false;
		}

	}
	
	public float Readfloat = 0;

	public boolean ReadFloat() {
		try {
			Readfloat = buf.getFloat();
			return true;
		} catch (Exception e) {
			return false;
		}
	}

	public byte[] Readbytes = null;

	public boolean ReadBytes() {
		try {
			int strlen = buf.getInt();
			byte[] strbytes = new byte[strlen];
			buf.get(strbytes);
			Readbytes = strbytes;
			return true;
		} catch (Exception e) {
			return false;
		}
	}

}
