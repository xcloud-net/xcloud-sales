import { PagedResponse, RoleDto, SysAdminDto } from '@/utils/models';
import u from '@/utils';
import { Alert, Button, Card, Checkbox, message, Modal, Space, Tag } from 'antd';
import { useEffect, useState } from 'react';

export default ({ model, ok }: { model: SysAdminDto, ok: any }) => {
  const [show, _show] = useState(false);
  const [selectedIds, _selectedIds] = useState<string[]>([]);
  const [loading, _loading] = useState(false);
  const [roles, _roles] = useState<RoleDto[]>([]);

  const queryRoles = () => {
    _loading(false);
    u.http.apiRequest.post<PagedResponse<RoleDto>>('/sys/role/paging', {
      Page: 1,
      PageSize: 100,
    }).then(res => {
      u.handleResponse(res, () => {
        _roles(res.data.Items || []);
      });
    }).finally(() => {
      _loading(false);
    });
  };

  useEffect(() => {
    show && queryRoles();
  }, [show]);

  const saveRoles = () => {
    _loading(false);
    u.http.apiRequest.post('/sys/role/save-admin-roles', {
      Id: model.Id,
      RoleIds: selectedIds,
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

  const renderRoles = () => {
    if (u.isEmpty(model.Roles)) {
      return null;
    }
    return <Space direction={'horizontal'}>
      {(model.Roles || []).map((x, i) => <Tag key={i}>{x.Name}</Tag>)}
    </Space>;
  };

  return <>
    <Space direction={'horizontal'}>
      {renderRoles()}
      <Button onClick={() => {
      }}>修改</Button>
    </Space>
    <Modal confirmLoading={loading} open={show} onCancel={() => {
    }} onOk={() => {
      saveRoles();
    }} okText={'确定'}>
      <Card loading={loading} extra={<Checkbox
        checked={roles.every(x => selectedIds.indexOf(x.Id || '') >= 0)}
        onChange={e => {
          if (e.target.checked) {
            _selectedIds(roles.map(x => x.Id || ''));
          } else {
            _selectedIds([]);
          }
        }}>全选</Checkbox>}>
        {u.isEmpty(roles) && <Alert>无角色</Alert>}
        {u.isEmpty(roles) || <div>
          {roles.map((x, i) => <Checkbox
            checked={selectedIds.indexOf(x.Id || '') >= 0}
            onChange={e => {
              var keys = selectedIds.filter(d => d != x.Id);
              if (e.target.checked) {
                keys = [...keys, x.Id || '.'];
              }
              _selectedIds(keys);
            }}
            style={{
              marginRight: 5,
              marginBottom: 5,
            }}
            key={i}
            value={x.Id}>{x.Name}</Checkbox>)}
        </div>}
      </Card>
    </Modal>
  </>;
};
