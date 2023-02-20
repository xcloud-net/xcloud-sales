import { RoleDto } from '@/utils/models';
import u from '@/utils';
import permission from '@/utils/permission';
import { Button, message, Modal, Space, Tag } from 'antd';
import { useState } from 'react';
import XPermissionForm from '../components/permission';

export default ({ model, ok }: { model: RoleDto, ok: any }) => {

  const [show, _show] = useState(false);

  const [loading, _loading] = useState(false);

  const savePermissions = (keys: string[]) => {
    _loading(true);
    u.http.apiRequest.post('/sys/role/save-role-permissions', {
      Id: model.Id,
      PermissionKeys: keys || [],
    }).then(res => {
      u.handleResponse(res, () => {
        message.success('保存成功');
        _show(false);
        ok && ok();
      });
    }).finally(() => {
      _loading(false);
    });
  };

  const renderPermissions = () => {
    var keys = (model.PermissionKeys || []);
    var pers = permission.all.filter(x => keys.indexOf(x.key) >= 0);
    if (u.isEmpty(pers)) {
      return null;
    }
    return <Space direction={'horizontal'}>
      {pers.map((x, i) => <Tag key={i}>{x.name}</Tag>)}
    </Space>;
  };

  return <>
    <div>
      <Space direction={'horizontal'}>
        {renderPermissions()}
        <Button onClick={() => {
          _show(true);
        }}>修改</Button>
      </Space>
      <Modal confirmLoading={loading} open={show} onCancel={() => {
        _show(false);
      }} okText={null}>
        <XPermissionForm loading={loading} keys={model.PermissionKeys} save={(keys: string[]) => {
          savePermissions(keys);
        }} />
      </Modal>
    </div>
  </>;
};
