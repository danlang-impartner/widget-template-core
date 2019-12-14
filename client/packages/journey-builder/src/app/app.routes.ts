import { Routes } from '@angular/router';

import { AccountsComponent } from './containers/accounts/accounts.component';
import { JourneysComponent } from './containers/journeys/journeys.component';

export const appRoutes: Routes = [
  { path: '', redirectTo: 'accounts', pathMatch: 'full' },
  {
    path: 'accounts',
    component: AccountsComponent
  },
  { path: 'journeys', component: JourneysComponent }
];
