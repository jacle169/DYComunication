package DYCom.J2SE;

import DYCom.J2SE.dyChangeListener;

public class dyDelegate {
	dyChangeListener listeners;

	public void setDYListener(dyChangeListener listen) {
		listeners = listen;
	}

	public void DYonconnectPropertyChanged(boolean result) {
		if(listeners !=null){
		listeners.onError(result);
	    }
	}
	
	public void DYondataPropertyChanged(byte[] m) {
		if(listeners !=null){
		listeners.onData(m);
		}
	}
 

}
