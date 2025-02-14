import { Component, ViewEncapsulation } from '@angular/core';
import { MenubarModule } from 'primeng/menubar';
import { CommonModule } from '@angular/common';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-header',
  imports: [MenubarModule, CommonModule],
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']  
})
export class HeaderComponent {
  items: MenuItem[] = [];

  constructor() {
  }

  ngOnInit() {
    this.items = [
      { label: 'Home', icon: 'pi pi-home', routerLink: ['/'] },
      { label: 'Add Fighter', icon: 'pi pi-box', routerLink: ['/addfighter'] },
      {
        label: 'Account',
        icon: 'pi pi-user',
        items: [
          { label: 'Profile', icon: 'pi pi-id-card', routerLink: ['/profile'] },
          { label: 'Settings', icon: 'pi pi-cog', routerLink: ['/settings'] }
        ]
      }
    ];
  }
}
