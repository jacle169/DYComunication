package DYCom.android;

public class dyDelegate {
	dyChangeListener listeners;

	public void setDYListener(dyChangeListener listen) {
		listeners = listen;
	}

	public void DYondataPropertyChanged(byte[] m) {
		listeners.onData(m);
	}

	public void DYonconnectPropertyChanged(boolean result) {
		listeners.onError(result);
	}
}

