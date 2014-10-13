package DYCom.android;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;

import org.apache.http.util.EncodingUtils;

public class DYwriter {
	public ByteBuffer Merge(ByteBuffer[] args) {
		int length = 0;
		for (ByteBuffer tempbyte : args) {
			length += tempbyte.array().length; // 计算数据包总长度
		}
		ByteBuffer bytebuf = ByteBuffer.allocate(length);
		bytebuf.order(ByteOrder.LITTLE_ENDIAN);

		for (ByteBuffer tempByte : args) {
			bytebuf.put(tempByte.array());
		}
		return bytebuf;
	}
	
	public ByteBuffer GetDYBytes(float values) {
		ByteBuffer bytebuf = ByteBuffer.allocate(4);
		bytebuf.order(ByteOrder.LITTLE_ENDIAN);
		bytebuf.putFloat(values);
		return bytebuf;
	}

	public ByteBuffer GetDYBytes(double values) {
		ByteBuffer bytebuf = ByteBuffer.allocate(8);
		bytebuf.order(ByteOrder.LITTLE_ENDIAN);
		bytebuf.putDouble(values);
		return bytebuf;
	}

	public ByteBuffer GetDYBytes(long values) {
		ByteBuffer bytebuf = ByteBuffer.allocate(8);
		bytebuf.order(ByteOrder.LITTLE_ENDIAN);
		bytebuf.putLong(values);
		return bytebuf;
	}

	public ByteBuffer GetDYBytes(short values) {
		ByteBuffer bytebuf = ByteBuffer.allocate(2);
		bytebuf.order(ByteOrder.LITTLE_ENDIAN);
		bytebuf.putShort(values);
		return bytebuf;
	}

	public ByteBuffer GetDYBytes(byte values) {
		ByteBuffer bytebuf = ByteBuffer.allocate(1);
		bytebuf.order(ByteOrder.LITTLE_ENDIAN);
		bytebuf.put(values);
		return bytebuf;
	}

	public ByteBuffer GetDYBytes(String values, String charter) {
		byte[] strMsg = EncodingUtils.getBytes(values, charter);
		ByteBuffer bytebuf = ByteBuffer.allocate(strMsg.length + 4);
		bytebuf.order(ByteOrder.LITTLE_ENDIAN);
		bytebuf.putInt(strMsg.length);
		bytebuf.put(strMsg);

		return bytebuf;
	}

	public ByteBuffer GetDYBytes(int values) {
		ByteBuffer bytebuf = ByteBuffer.allocate(4);
		bytebuf.order(ByteOrder.LITTLE_ENDIAN);
		bytebuf.putInt(values);
		return bytebuf;
	}

	public ByteBuffer GetDYBytes(byte[] values) {
		ByteBuffer bytebuf = ByteBuffer.allocate(values.length + 4);
		bytebuf.order(ByteOrder.LITTLE_ENDIAN);
		bytebuf.putInt(values.length);
		bytebuf.put(values);
		return bytebuf;
	}
}
