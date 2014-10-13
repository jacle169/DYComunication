package DYCom.android;

/***********************************************************************
 * Project: DYCom Android1.5 Client
 * Module: Beta1.0.0.1
 * Author: DonghaiLi http://dy2com.com
 ***********************************************************************/
import java.io.*;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.nio.*;

import android.os.Message;

public class clientPoxy implements Runnable {
	private Socket socket = null;
	private OutputStream out = null;
	private InputStream in = null;
	private ByteBuffer buffers = null;
	private int buffersize = 256;
	public dyOndataHandler Ondatahandler = null;
	public dyOnErrorHandler connectHander = null;

	public clientPoxy(String serverIP, int port, int bufferSize, int connectTimeout, int SendTimeout) {
		Ondatahandler = new dyOndataHandler();
		connectHander = new dyOnErrorHandler();
		buffersize = bufferSize;
		buffers = ByteBuffer.allocate(buffersize);
		buffers.order(ByteOrder.LITTLE_ENDIAN);
		try {
			socket = new Socket();
			socket.setSoLinger(true, SendTimeout);
			socket.setSoTimeout(SendTimeout * 1000);
			socket.connect(new InetSocketAddress(serverIP, port),
					connectTimeout * 1000);
			out = socket.getOutputStream();
			in = socket.getInputStream();
			Message msg = new Message();
			msg.obj = true;
			if (connectHander != null) {
				connectHander.sendMessage(msg);
			}
			canRead = true;
			new Thread(this).start();
		} catch (Exception e) {
			dis();
		}
	}

	public int readInt(byte[] bytes) {
		int num = bytes[0] & 0xFF;
		num |= ((bytes[1] << 8) & 0xFF00);
		num |= ((bytes[2] << 16) & 0xFF0000);
		num |= ((bytes[3] << 24) & 0xFF000000);
		return num;
	}

	byte[] buf = null;
	boolean canRead = false;

	public void run() {
		try {
			while (canRead) {
				if (socket.isConnected()) {
					if (!socket.isInputShutdown()) {
						if (in.available() > 0) {
							buf = new byte[in.available()];
							in.read(buf, 0, buf.length);
							innerData(buf);
						}
					} else {
						dis();
					}
				} else {
					dis();
				}
				Thread.sleep(1);
			}
		} catch (Exception ex) {
			dis();
		}
	}

	void dis() {
		try {
			canRead = false;
			socket.shutdownInput();
			socket.shutdownOutput();
			socket.close();
		} catch (Exception e) {
		}
		Message msg = new Message();
		msg.obj = false;
		if (connectHander != null) {
			connectHander.sendMessage(msg);
		}
	}

	void innerData(byte[] buf) {
		buffers.put(buf);

		buffers.flip();
		byte[] content = new byte[buffers.limit()];
		buffers.get(content);

		if (readData(content)) {
			canRead = false;
			buffers = ByteBuffer.allocate(buffersize);
			buffers.order(ByteOrder.LITTLE_ENDIAN);
			canRead = true;

			ByteBuffer read = ByteBuffer.wrap(content);
			read.order(ByteOrder.LITTLE_ENDIAN);

			int len = read.getInt() - 4;
			byte[] m = new byte[len];
			read.get(m);

			Message msg = new Message();
			msg.obj = m;
			if (Ondatahandler != null) {
				Ondatahandler.sendMessage(msg);
			}

		}
	}

	boolean readData(byte[] data) {
		ByteBuffer reader = ByteBuffer.wrap(data);
		reader.order(ByteOrder.LITTLE_ENDIAN);
		int ml = reader.getInt();
		if (data.length >= ml) {
			return true;
		} else {
			return false;
		}
	}

	public void send(ByteBuffer msg) {
		try {
			int tempLength = msg.array().length + 4;
			ByteBuffer bytebuf = ByteBuffer.allocate(tempLength);
			bytebuf.order(ByteOrder.LITTLE_ENDIAN);
			bytebuf.putInt(tempLength);
			bytebuf.put(msg.array());
			out.write(bytebuf.array());
			out.flush();
		} catch (Exception e) {
			dis();
		}
	}

}
