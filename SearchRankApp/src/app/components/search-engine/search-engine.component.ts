import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatSelectModule } from '@angular/material/select';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatDividerModule } from '@angular/material/divider';
import { MatIconModule } from '@angular/material/icon';
import { BreadcrumbComponent } from '../../shared/breadcrumb.component';
import { SearchEngine } from './search-engine.enum';
import { SearchEngineService } from '../../search-rank-api/services';
import { ILogger } from '../../services/logger/logger.interface';
import { LOGGER_TOKEN } from '../../services/logger/logger.token';

@Component({
  standalone: true,
  selector: 'app-search',
  templateUrl: './search-engine.component.html',
  styleUrls: ['./search-engine.component.scss'],
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatFormFieldModule,
    MatInputModule,
    MatSelectModule,
    MatButtonModule,
    MatCardModule,
    MatDividerModule,
    MatIconModule,
    BreadcrumbComponent
  ],
})
export class SearchEngineComponent {
  searchForm: FormGroup;
  searchEngines = this.getSearchEngineOptions();
  result: any;

  private readonly searchService = inject(SearchEngineService);
  private readonly logger = inject(LOGGER_TOKEN) as ILogger;
  private readonly fb = inject(FormBuilder);

  constructor() {
    this.searchForm = this.fb.group({
      keywords: ['', Validators.required],
      url: ['', [Validators.required, Validators.pattern('https?://.+')]],
      searchEngine: [SearchEngine.Google, Validators.required],
    });
  }

  onSubmit(): void {
    if (this.searchForm.invalid) return;

    const { keywords, url, searchEngine } = this.searchForm.value;

    const searchMethods = {
      [SearchEngine.Google]: () =>
        this.searchService.apiGoogleSearchRankingGet$Response({ Keyword: keywords, TargetUrl: url }),
      [SearchEngine.Bing]: () =>
        this.searchService.apiBingSearchRankingGet$Response({ Keyword: keywords, TargetUrl: url }),
    };

    const searchCall = searchMethods[searchEngine as SearchEngine];

    if (!searchCall) {
      this.logger.warn(`Unsupported search engine: ${searchEngine}`);
      return;
    }

    searchCall().subscribe({
      next: (response) => {
        this.result = response.body.result;
      },
      error: (err) => this.logger.error(`Search failed: ${err}`),
    });
  }

  getSelectedEngineLabel(): string {
    const selectedEngineValue = this.searchForm.get('searchEngine')?.value;
    const selectedEngine = this.searchEngines.find(e => e.value === selectedEngineValue);
    return selectedEngine?.label || 'Unknown';
  }

  onReset(): void {
    this.searchForm.reset({
      keywords: '',
      url: '',
      searchEngine: SearchEngine.Google
    });
  
    this.result = null;
  }
  private getSearchEngineOptions(): { label: string; value: number }[] {
    return Object.keys(SearchEngine)
      .filter((key) => isNaN(Number(key)))
      .map((key) => ({
        label: key,
        value: SearchEngine[key as keyof typeof SearchEngine],
      }));
  }
}
