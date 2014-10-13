package DYConJ2SETextClient.test;

import java.nio.ByteBuffer;
import DYCom.J2SE.*;

public class testclient implements dyChangeListener {
	// ���������
	static int login = 0;
	// DYCom�ͻ��˶���
	static clientPoxy poxy;
	// DYcom��Ϣ����������
	static DYwriter dm;

	/**
	 * @param args
	 */
	public static void main(String[] args) {
		// TODO Auto-generated method stub
		// ʵ����DYCom��Ϣ������
		dm = new DYwriter();
		// ʵ����DYCom�ͻ��ˣ�������IP,�˿�,��������С��
		poxy = new clientPoxy("test.dy2com.com", 4510, 1024);
		// ע���¼�������
		poxy.dyevent.setDYListener(new testclient());

		// json���л�����
		// JSONObject json=new JSONObject();
		// try {
		//�������ݵ�json
		// json.put("aa", "jac");
		// json.put("bb", 13);
		// } catch (JSONException e) {
		// TODO Auto-generated catch block
		// e.printStackTrace();
		// }
		// ����json����
		// ByteBuffer[] msg ={dm.GetDYBytes(login),dm.GetDYBytes(json.toString(),"UTF-8")};
		// poxy.send(dm.Merge(msg));

		// ������Ϣ׼�����͵�������
		ByteBuffer[] msg = { dm.GetDYBytes(login),dm.GetDYBytes("DYCOM For J2SE!", "UTF-8") };
		// �����û�����EditText����
		poxy.send(dm.Merge(msg));

	}

	@Override
	public void onError(boolean result) {
		// TODO Auto-generated method stub
		// ��ʾ���ӽ��
		display(Boolean.toString(result));
	}

	// ���ݵ��������¼�
	@Override
	public void onData(byte[] data) {
		// ʵ����DYCOM��Ϣ������
		DYreader read = new DYreader(data);
		// ����������
		int type = -1;
		// ��ȡ��Ϣ�еĲ�����
		if (!read.ReadInt32()) {
			// �����Ϣ�����Ϸ�ֱ�Ӳ����κδ���
			return;
		}
		// �ӽ������ж�����Ϣ��
		type = read.Readint32;
		// �жϲ�����
		if (type == login) {
			// ���Զ���һ���ַ���ֵ,ʹ��UTF8����
			if (read.ReadString("UTF-8")) {
				// ��ʾ�Ѷ����ַ���ֵ
				display(read.Readstring);

				// ��ȡ��������
				// if(read.ReadString("UTF-8"))
				// {
				// display(read.Readstring);
				// }

				// json���ݶ�ȡ����
				// JSONObject jsonObject;
				// try {
				// jsonObject = new JSONObject(read.Readstring);
				// display(jsonObject.getString("aa"));
				// display(jsonObject.getString("bb"));
				// } catch (JSONException e) {
				// TODO Auto-generated catch block
				// e.printStackTrace();
				// }

			}
		}
	}

	void display(String msg) {
		System.out.println(msg + "\n");
	}

}
