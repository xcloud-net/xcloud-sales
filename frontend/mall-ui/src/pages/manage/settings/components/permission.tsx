import permissions from '@/utils/permission/data';
import { Button, Card, Checkbox } from 'antd';
import { useState } from 'react';
import { Permission, PermissionGroup } from '@/utils/models';
import u from '@/utils';

export default () => {
  const [selectedKeys, _selectedKeys] = useState<any[]>([]);

  const renderCheckbox = (d: Permission) => {
    return (
      <Checkbox
        checked={selectedKeys.indexOf(d.key) >= 0}
        onChange={(e) => {
          var items = selectedKeys.filter((x) => x != d.key);
          if (e.target.checked) {
            items.push(d.key);
          }
          _selectedKeys(items);
        }}
      >
        {d.name}
      </Checkbox>
    );
  };

  const renderGroup = (x: PermissionGroup) => {
    var all = x.permissions.every((d) => selectedKeys.indexOf(d.key) >= 0);

    return (
      <Card
        title={x.name}
        style={{ marginBottom: 10 }}
        bordered={false}
        extra={
          <Checkbox
            checked={all}
            onChange={(e) => {
              var allGroupPermissions = x.permissions.map((x) => x.key);
              var items = selectedKeys.filter(
                (x) => allGroupPermissions.indexOf(x) < 0,
              );
              if (e.target.checked) {
                items = [...items, ...allGroupPermissions];
              }
              _selectedKeys(items);
            }}
          >
            全选
          </Checkbox>
        }
      >
        {x.permissions.map((d, i) => {
          return (
            <span style={{ marginRight: 10, marginBottom: 10 }} key={i}>
              {renderCheckbox(d)}
            </span>
          );
        })}
      </Card>
    );
  };

  return (
    <>
      <Card
        extra={<Button type="primary">保存</Button>}
        style={{
          backgroundColor: 'rgb(250,250,250)',
        }}
      >
        {permissions.groups.map((x, index) => {
          return <div key={index}>{renderGroup(x)}</div>;
        })}
      </Card>
    </>
  );
};
