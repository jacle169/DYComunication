package DYCom.android;

import android.os.Handler;
import android.os.Looper;
import android.os.Message;

public class dyOndataHandler extends Handler {
	public dyDelegate dyevent = new dyDelegate();

	public dyOndataHandler() {
	}

	public dyOndataHandler(Looper L) {
		super(L);
	}

	@Override
	public void handleMessage(Message msg) {
		super.handleMessage(msg);
		byte[] data = (byte[]) msg.obj;
		dyevent.DYondataPropertyChanged(data);
	}
}
