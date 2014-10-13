package DYCom.android;

import android.os.Handler;
import android.os.Looper;
import android.os.Message;

public class dyOnErrorHandler extends Handler {
	public dyDelegate dyevent = new dyDelegate();

	public dyOnErrorHandler() {
	}

	public dyOnErrorHandler(Looper L) {
		super(L);
	}

	@Override
	public void handleMessage(Message msg) {
		super.handleMessage(msg);
		boolean data = (Boolean) msg.obj;
		dyevent.DYonconnectPropertyChanged(data);
	}
}
