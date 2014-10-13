package DYCom.android;

public interface dyChangeListener {
	public void onError(boolean result);

	public void onData(byte[] data);
}
