import { PlusSquareOutlined } from '@ant-design/icons';
import { Badge, Button, Modal, message } from 'antd';
import { useState } from 'react';
import u from '@/utils';

const AdminPage = (props: any) => {
  const { model, ok } = props;
  const { AdminIdentity } = model;

  const [show, _show] = useState(false);
  const [loading, _loading] = useState(false);

  const setAsAdmin = () => {
    if (!confirm('确定？')) {
      return;
    }
    _loading(true);
    u.http.adminRequest
      .post('/admin/add', {
        UserId: model.Id,
      })
      .then((res) => {
        if (res.data.Error) {
          message.error(res.data.Error.Message);
        } else {
          message.success('添加成功');
          ok && ok();
        }
      })
      .finally(() => {
        _loading(false);
      });
  };

  if (AdminIdentity) {
    return <Badge status="success" text="管理员" />;
  } else {
    return (
      <>
        <Modal
          title="设置为管理员"
          open={show}
          onCancel={() => {
            _show(false);
          }}
          onOk={() => {
            setAsAdmin();
          }}
          confirmLoading={loading}
        >
          <h4>确定把当前用户设置为系统管理员吗？</h4>
        </Modal>
        <Button
          icon={<PlusSquareOutlined />}
          onClick={() => {
            _show(true);
          }}
        ></Button>
      </>
    );
  }
};

export default AdminPage;
